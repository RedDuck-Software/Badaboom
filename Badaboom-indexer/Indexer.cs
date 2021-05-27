using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Badaboom_indexer.Extensions;
using Database.Models;
using Database.Respositories;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace Badaboom_indexer
{
    public class Indexer
    {
        private Web3 _web3;

        public ulong LatestBlockNumber { get; private set; }


        public static async Task<Indexer> CreateInstance(Web3 web3)
        {
            ulong lastBlockNumber = (await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();

            return new Indexer(web3, lastBlockNumber);
        }

        private Indexer(Web3 web3, ulong lastBlockNumber)
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
                        using (var repo = new TransactionRepository())
                        {
                            await repo.AddNewTransactionAsync(tx);
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
                string value = t.Value.HexValue;

                return new Transaction
                {
                    ContractAddress = t.From,
                    Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.ToUlong()).UtcDateTime,
                    Hash = t.TransactionHash,
                    MethodId = value.Substring(0, value.Length > 10 ? 10 : value.Length)
                };
            });
        }
    }
}
