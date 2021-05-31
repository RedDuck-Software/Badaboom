using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public ulong LatestBlockNumber => _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result.ToUlong();

        public Indexer(Web3Parity web3)
        {
            _web3 = web3;
        }

        public async Task IndexInRangeParallel(ulong startBlock, ulong endBlock, int tasksCount = 1)
        {
            List<Task> tasks = new List<Task>();

            ulong step = (endBlock - startBlock) / (ulong)tasksCount;

            ConsoleColor.Magenta.WriteLine($"Indexing proccess started with {tasksCount} tasks\n");

            for (int i = 0; i < tasksCount; i++)
                tasks.Add(this.IndexInRange(startBlock + step * (ulong)i, startBlock + step * (ulong)(i + 1)));

            if (step * (ulong)tasksCount + startBlock < endBlock)
                tasks.Add(this.IndexInRange(startBlock + step * (ulong)(tasksCount), endBlock));

            await Task.WhenAll(tasks);
        }

        public async Task StartMonitorNewBlocks()
        {
            while (true)
            {
                Block lastBlockRecorded;

                using (var bRepo = new BlocksRepository())
                    lastBlockRecorded = await bRepo.GetLastSuccessfulBlockAsync() ?? new() { BlockNumber = 0 };

                await this.IndexInRangeParallel(
                    (ulong)lastBlockRecorded.BlockNumber,
                    this.LatestBlockNumber,
                    (ulong)lastBlockRecorded.BlockNumber - this.LatestBlockNumber > 100 ? 10 : 1);

                Thread.Sleep(1000 * 12); // 12 seconds before next indexing
            }
        }

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            if (startBlock >= endBlock) throw new ArgumentException("Start block cannot be bigger then end block");

            ConsoleColor.Cyan.WriteLine($"Blocks [{startBlock}] - [{endBlock}]");


            for (ulong i = startBlock; i < endBlock; i++)
            {
                try
                {
                    var txs = await _getBlockTransactions(i);

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

                        if (txInserted is null)
                        {
                            ConsoleColor.DarkYellow.WriteLine($"Tx {tx.Hash} is already indexed");
                            continue;
                        }

                        var parityTraceRaw = (await _web3.Trace.TraceTransaction.SendRequestAsync(tx.Hash));


                        if (parityTraceRaw is null)
                        {
                            using (var cRepo = new TransactionRepository())
                                await cRepo.AddNewCallAsync(
                                    new Call()
                                    {
                                        ContractAddress = tx.RawTransaction.ContractAddress,
                                        MethodId = tx.RawTransaction.MethodId,
                                        From = tx.RawTransaction.From,
                                        Value = tx.RawTransaction.Value
                                    }) ;

                            ConsoleColor.Gray.WriteLine($"Unnable to get trace of transaction {tx.Hash}, skiping internal calls");

                            continue;
                        }

                        var txParityTraces = parityTraceRaw.ToObject<IEnumerable<ParityTrace>>();


                        foreach (var trace in txParityTraces)
                        {
                            if (!(trace.Type == "create" || trace.Type == "call"))
                            {
                                ConsoleColor.DarkBlue.WriteLine($"Transactions with callType is not {trace.Type} included");
                                continue;
                            }

                            using (var cRepo = new TransactionRepository())
                            {
                                await cRepo.AddNewCallAsync(
                                    new Call
                                    {
                                        From = trace.Action.From ?? "",
                                        ContractAddress = trace.Action.To ?? "",
                                        MethodId = _getMethodIdFromInput(trace.Action.Input),
                                        TransactionId = txInserted.Id,
                                        Value = trace.Action.Value ?? "" 
                                    });
                            }
                        }

                        ConsoleColor.Blue.WriteLine(tx.Hash);
                    }


                    using (var bRepo = new BlocksRepository())
                    {
                        await bRepo.AddNewSuccessBlockAsync(new Block
                        {
                            BlockNumber = (long)i
                        });
                    }

                    ConsoleColor.Green.WriteLine($"Block {i} indexed");
                }
                catch (Exception ex)
                {
                    using (var repo = new BlocksRepository())
                    {
                        await repo.AddNewFailedBlockAsync(new Block { BlockNumber = (long)i });
                    }

                    ConsoleColor.Red.WriteLine($"GetBlockTransactions() Failed on block {i}");
                }
            }
        }

        private async Task<IEnumerable<Transaction>> _getBlockTransactions(ulong blockNubmer)
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
                        From = t.From ?? "",
                        ContractAddress = t.To ?? "",
                        MethodId = _getMethodIdFromInput(input),
                        Value = t.Value?.ToString() ?? ""
                    }
                };
            });
        }

        /*
                private int _getOptimalTasksCountForRange(uint range)
                {
                    if (range >= 0 && range <= 100) return 1;
                    if(range > 100 && range <= 1000) return

                }*/

        private string _getMethodIdFromInput(string value)
        {
            if (value is null) return String.Empty;
            return value.Substring(0, value.Length > 10 ? 10 : value.Length);
        }
    }
}
