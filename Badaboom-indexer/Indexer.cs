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

        public Indexer(Web3 web3)
        {
            _web3 = web3;
        }



        public async Task IndexInRangeParallel(ulong startBlock, ulong endBlock, int tasksCount = 1)
        {
            List<Task> tasks = new List<Task>();

            ulong step = (endBlock - startBlock) / (ulong)tasksCount;

            ConsoleColor.Magenta.WriteLine("Indexing proccess started!\n");

            for (int i = 0; i < tasksCount; i++)
            {
                tasks.Add(this.IndexInRange(startBlock + step * (ulong)i, startBlock + step * (ulong)(i + 1)));
            }

            await Task.WhenAll(tasks);
        } 

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            if (startBlock >= endBlock) throw new ArgumentException("start block cannot be bigger then end block");

            for (ulong i = startBlock; i < endBlock; i++)
            {
                if (i % 100 == 0 || i == 0)
                    ConsoleColor.Cyan.WriteLine($"Blocks [{i}] - [{(i + 100 <= endBlock ? i + 100 : endBlock)}]");

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
                    Time = UnixTimeToDateTime(block.Timestamp.ToUlong()),
                    Hash = t.TransactionHash,
                    MethodId = value.Substring(0, value.Length > 10 ? 10 : value.Length)
                };
            });
        }

        public DateTime UnixTimeToDateTime(ulong unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
}
