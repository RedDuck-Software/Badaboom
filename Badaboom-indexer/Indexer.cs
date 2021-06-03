using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Badaboom_indexer.Extensions;
using Badaboom_indexer.Models;
using Database.Models;
using Database.Respositories;
using Nethereum.Hex.HexTypes;
using Nethereum.Parity;

namespace Badaboom_indexer
{
    public class Indexer
    {
        private Web3Parity _web3;

        public Indexer(Web3Parity web3)
        {
            _web3 = web3;
        }

        public async Task<ulong> GetLatestBlockNumber() => (await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();

        public async Task IndexInRangeParallel(ulong startBlock, ulong endBlock, ulong step = 20)
        {
            if (startBlock >= endBlock) throw new ArgumentException("Start block cannot be bigger then end block");

            ConsoleColor.Magenta.WriteLine($"Indexing proccess started. Step - 20\n");

            var movements = (endBlock - startBlock) / step;

            for (ulong i = 0; i < movements; i += step)
                await this.IndexInRange(i + startBlock, i + step + startBlock);

            var multRes = startBlock + step * movements;

            if (multRes < endBlock)
                await this.IndexInRange(multRes, endBlock);
        }


        /// <summary>
        /// Starts monitor for new blocks. From latest block recorded in DB to Latest Block available in the Node
        /// </summary>
        /// <returns></returns>
        public async Task StartMonitorNewBlocks()
        {
            while (true)
            {
                Block lastBlockRecorded;

                using (var bRepo = new BlocksRepository())
                    lastBlockRecorded = await bRepo.GetLastIndexedBlockAsync() ?? new() { BlockNumber = 0 };

                await this.IndexInRangeParallel(
                    (ulong)lastBlockRecorded.BlockNumber,
                    await this.GetLatestBlockNumber(),
                    20);

                await Task.Delay(TimeSpan.FromSeconds(12));
            }
        }

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            ConsoleColor.Cyan.WriteLine($"Blocks [{startBlock}] - [{endBlock}]");

            List<Task> tasks = new();

            for (ulong i = startBlock; i < endBlock; i++)
                tasks.Add(IndexBlock(i));

            await Task.WhenAll(tasks);
        }

        private async Task IndexBlock(ulong blockNubmer)
        {
            if (await this.ContainsSuccessfulBlock(new Block { BlockNumber = (long)blockNubmer }))
            {
                ConsoleColor.Yellow.WriteLine($"Block {blockNubmer} is already indexed. Skipping");
                return;
            }


            try
            {
                using (var bRepo = new BlocksRepository())
                    await bRepo.AddNewBlockAsync(new Block { BlockNumber = (long)blockNubmer }, BlocksRepository.BlockStatus.INDEXING);


                var txs = await GetBlockTransactions(blockNubmer);

                if (!txs.Any())
                {
                    ConsoleColor.Yellow.WriteLine($"no transactions in block {blockNubmer}. Skipping");
                }
                else
                {
                    foreach (var tx in txs)
                    {
                        await IndexTransaction(tx);
                    }
                }

                using (var bRepo = new BlocksRepository())
                    await bRepo.ChangeBlockStatusTo(new Block { BlockNumber = (long)blockNubmer }, BlocksRepository.BlockStatus.INDEXED);

                ConsoleColor.Green.WriteLine($"Block {blockNubmer} indexed");
            }
            catch (Exception ex)
            {
                using (var repo = new BlocksRepository())
                {
                    var block = new Block { BlockNumber = (long)blockNubmer };

                    if (await ContainsSuccessfulBlock(block))
                        await repo.ChangeBlockStatusTo(block, BlocksRepository.BlockStatus.FAILED);
                    else
                        await repo.AddNewBlockAsync(block, BlocksRepository.BlockStatus.FAILED);
                }

                ConsoleColor.Red.WriteLine($"GetBlockTransactions() Failed on block {blockNubmer}. Ex: {ex}");
            }
        }

        private async Task IndexTransaction(Transaction tx)
        {
            Transaction txInserted;

            try
            {
                using var repo = new TransactionRepository();
                txInserted = await repo.AddNewTransactionAsync(tx);
            }
            catch (SqlException ex)
            {
                ConsoleColor.DarkYellow.WriteLine($"SqlException on {tx.Hash}. ExMessage: {ex.Message}");
                return;
            }

            var parityTraceRaw = (await _web3.Trace.TraceTransaction.SendRequestAsync(tx.Hash));

            if (parityTraceRaw is null)
            {
                try
                {
                    await this.AddNewCallAsync(
                        new Call()
                        {
                            TransactionId = txInserted.Id,
                            ContractAddress = tx.RawTransaction.ContractAddress,
                            MethodId = tx.RawTransaction.MethodId,
                            From = tx.RawTransaction.From,
                            Value = tx.RawTransaction.Value
                        });

                    ConsoleColor.Gray.WriteLine($"Unnable to get trace of transaction {tx.Hash}, skiping internal calls");
                }
                catch (SqlException ex)
                {
                    ConsoleColor.Red.WriteLine($"SqlException occured when tried to add Tx {tx.Hash}. ExMessage: {ex.Message}");
                }

                return;
            }

            var txParityTraces = parityTraceRaw.ToObject<IEnumerable<ParityTrace>>();

            foreach (var trace in txParityTraces)
            {
                await IndexCall(trace, txInserted);
            }

            ConsoleColor.Green.WriteLine(tx.Hash);
        }

        private async Task IndexCall(ParityTrace trace, Transaction tx)
        {
            if (!(trace.Type == "create" || trace.Type == "call"))
            {
                ConsoleColor.DarkBlue.WriteLine($"Transactions with callType {trace.Type} is not included");
                return;
            }

            ConsoleColor.DarkBlue.WriteLine($"\t{trace.Type}");

            try
            {
                await this.AddNewCallAsync(
                    new Call
                    {
                        From = trace?.Action?.From,
                        ContractAddress = trace?.Action?.To,
                        MethodId = _getMethodIdFromInput(trace?.Action?.Input),
                        TransactionId = tx.Id,
                        Value = trace?.Action?.Value,
                        Type = trace.Type
                    });
            }
            catch (SqlException ex)
            {
                ConsoleColor.Red.WriteLine(
                    $"SqlException occured when tried to add call {trace.Action.From} - from, {trace?.Action?.To} - to ." +
                    $"TxHash: {tx.Hash}. " +
                    $"ExMessage: {ex.Message}");
            }
        }


        private async Task<bool> ContainsSuccessfulBlock(Block block)
        {
            using var bRepo = new BlocksRepository();
            return await bRepo.ContainsBlockAsync(block);
        }

        private async Task AddNewCallAsync(Call call)
        {
            using var cRepo = new TransactionRepository();
            await cRepo.AddNewCallAsync(call);
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
                        ContractAddress = t.To, // todo: research {meaning of contractAddress; empty to address}
                        MethodId = _getMethodIdFromInput(input),
                        Value = t.Value?.ToString()
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