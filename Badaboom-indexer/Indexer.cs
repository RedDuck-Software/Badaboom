using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Badaboom_indexer.Extensions;
using Badaboom_indexer.Models;
using Database.Models;
using Database.Respositories;
using Nethereum.Hex.HexTypes;
using Nethereum.Parity;
using Nethereum.Web3;
using Newtonsoft.Json;

namespace Badaboom_indexer
{
    public class Indexer
    {
        private Web3Parity _web3;

        public ulong LatestBlockNumber { get; private set; }


        public static async Task<Indexer> CreateInstance(Web3Parity web3)
        {
            ulong lastBlockNumber = (await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();

            return new Indexer(web3, lastBlockNumber);
        }

        private Indexer(Web3Parity web3, ulong lastBlockNumber)
        {
            _web3 = web3;
            LatestBlockNumber = lastBlockNumber;
        }

        public async Task IndexInRangeParallel(ulong startBlock, ulong endBlock, int tasksCount = 1)
        {
            List<Task> tasks = new List<Task>();

            ulong step = (endBlock - startBlock) / (ulong)tasksCount;

            ConsoleColor.Magenta.WriteLine("Indexing proccess started!\n");

            for (int i = 0; i < tasksCount; i++)
                tasks.Add(this.IndexInRange(startBlock + step * (ulong)i, startBlock + step * (ulong)(i + 1)));

            if (step * (ulong)tasksCount + startBlock < endBlock)
                tasks.Add(this.IndexInRange(startBlock + step * (ulong)(tasksCount), endBlock));

            await Task.WhenAll(tasks);
        }

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            if (startBlock >= endBlock) throw new ArgumentException("start block cannot be bigger then end block");

            ConsoleColor.Cyan.WriteLine($"Blocks [{startBlock}] - [{endBlock}]");


            for (ulong i = startBlock; i < endBlock; i++)
            {
                try
                {
                    var txs = await GetBlockTransactions(i);

                    if (txs.Count() == 0)
                    {
                        ConsoleColor.Yellow.WriteLine($"0 transactions in block {i}. Skipping");
                        continue;
                    }

                    foreach (var tx in txs)
                    {
                        Transaction txInserted;

                        using (var repo = new TransactionRepository())
                        {
                            txInserted = await repo.AddNewTransactionAsync(tx);
                        }

                        if (txInserted is null) continue;

                        var parityTraceRaw = (await _web3.Trace.TraceTransaction.SendRequestAsync(tx.Hash));


                        if (parityTraceRaw is null)
                        {
                            using (var cRepo = new TransactionRepository())
                                await cRepo.AddNewCallAsync(
                                    new Call() 
                                    { 
                                        ContractAddress = tx.RawTransaction.ContractAddress, 
                                        MethodId = tx.RawTransaction.MethodId,
                                        From = tx.RawTransaction.From 
                                    });

                            ConsoleColor.Gray.WriteLine($"Unnable to get trace of transaction {tx.Hash}, skiping internal calls");

                            continue;
                        }

                        var txParityTraces = parityTraceRaw.ToObject<IEnumerable<ParityTrace>>();


                        foreach (var trace in txParityTraces)
                        {
                            using (var cRepo = new TransactionRepository())
                            {
                                await cRepo.AddNewCallAsync(
                                    new Call
                                    { 
                                        From = trace.Action.From,
                                        ContractAddress = trace.Action.To,
                                        MethodId = _getMethodIdFromInput(trace.Action.Input),
                                        TransactionId = txInserted.Id
                                    });
                            }
                        }

                        ConsoleColor.Blue.WriteLine(tx.Hash);
                    }
                }
                catch (Exception ex)
                {
                    using (var repo = new FailedBlocksRepository())
                    {
                        await repo.AddNewFailedBlockAsync(new FailedBlock { BlockNumber = (long)i });
                    }

                    ConsoleColor.Red.WriteLine($"GetBlockTransactions() Failed on block {i}");
                }
            }
        }

        private async Task<IEnumerable<Transaction>> GetBlockTransactions(ulong blockNubmer)
        {
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNubmer.ToHexBigInteger());

            return block.Transactions.Select(t =>
            {
                string input = t.Value.HexValue;

                return new Transaction
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.ToUlong()).UtcDateTime,
                    Hash = t.TransactionHash,
                    RawTransaction = new RawTransaction
                    {
                        From = t.From,
                        ContractAddress = t.To,
                        MethodId = _getMethodIdFromInput(input),
                    }
                };
            });
        }

        private string _getMethodIdFromInput(string value)
        {
            if (value is null) return String.Empty;
            return value.Substring(0, value.Length > 10 ? 10 : value.Length);
        }
    }
}
