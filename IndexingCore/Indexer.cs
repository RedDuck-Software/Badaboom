using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using IndexerCore.Extensions;
using Database.Respositories;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Web3Tracer.Tracers;
using System.Collections.Concurrent;

namespace IndexerCore
{
    public class Indexer
    {
        public Web3 Web3 => _web3Tracer.Web3;

        public IReadOnlyCollection<string> ValidCallTypes { get; } = new string[]
                { "create", "call", "delegatecall", "staticcall" , "callcode" };

        public ConcurrentQueue<Block> BlockQueue { get; set; }

        public readonly int QueueSize;

        public const int MAX_INSERT_ROWS = 1000;


        public Indexer(IWeb3Tracer web3Tracer, string dbConnectionString, int queueSize = MAX_INSERT_ROWS)
        {
            _web3Tracer = web3Tracer;
            _connectionString = dbConnectionString;
            QueueSize = queueSize;
        }

        public async Task<ulong> GetLatestBlockNumber() => (await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();

        public async Task IndexFailedAndPendingBlocks()
        {
            using (var bRepo = new BlocksRepository(_connectionString))
            {
                var failedBlocks = await bRepo.GetAllFailedBlocksAsync();

                if (failedBlocks is null) failedBlocks = new List<Block>();

                var pendingBlocks = await bRepo.GetAllPendingBlocksAsync();

                if (pendingBlocks is null) pendingBlocks = new List<Block>();

                var blocks = failedBlocks.Concat(pendingBlocks);

                ConsoleColor.Green.WriteLine($"Indexing {failedBlocks.Count()} failed blocks and {pendingBlocks.Count()} locked in pending blocks\n\n");


                foreach (var block in blocks)
                {
                    await bRepo.RemoveBlockAsync(block);
                    await IndexBlock((ulong)block.BlockNumber);
                }
            }

            ConsoleColor.Magenta.WriteLine("Indexing completed");
        }

        public async Task IndexInRangeParallel(ulong startBlock, ulong endBlock, ulong step = 20)
        {
            if (startBlock >= endBlock) throw new ArgumentException("Start block cannot be bigger then end block");

            ConsoleColor.Magenta.WriteLine($"Indexing proccess started. Step - 20\n");

            var movements = (endBlock - startBlock) / step;

            for (ulong i = 0, j = 0; j < movements; i += step, j++)
                await this.IndexInRange(i + startBlock, i + step + startBlock);

            var multRes = startBlock + step * movements;

            if (multRes < endBlock)
                await this.IndexInRange(multRes, endBlock);
        }

        public async Task CommitIndexedBlocks()
        {
            ConsoleColor.Green.WriteLine($"Commiting queued blocks. Current queue size is: { BlockQueue.Count}");

            // split queue into chunks because of t-sql limitation in 1000 rows per one sql query
            IEnumerable<IEnumerable<Block>> blockQueueChunked = ChunkBy(BlockQueue.ToList(), MAX_INSERT_ROWS);

            using (var bRepo = new BlocksRepository(_connectionString))
            {
                using (var tRepo = new TransactionRepository(_connectionString))
                {
                    foreach (var chunk in blockQueueChunked)
                    {
                        await bRepo.AddNewBlocksAsync(chunk);

                        foreach (var block in chunk)
                        {
                            await tRepo.AddNewTransactionsAsync(block.Transactions);

                            foreach (var tx in block.Transactions)
                                await tRepo.AddNewCallsAsync(tx.Calls);
                        }
                    }
                }
            }

            BlockQueue.Clear();
        }

        /// <summary>
        /// Starts monitor for new blocks. From latest block recorded in DB to Latest Block available in the Node
        /// </summary>
        /// <returns></returns>
        public async Task StartMonitorNewBlocks(double secondsToSleepBeforeIterations)
        {
            while (true)
            {
                Block lastBlockRecorded;

                using (var bRepo = new BlocksRepository(_connectionString))
                    lastBlockRecorded = await bRepo.GetLastIndexedBlockAsync() ?? new() { BlockNumber = 0 };

                await this.IndexInRangeParallel(
                    (ulong)lastBlockRecorded.BlockNumber,
                    await this.GetLatestBlockNumber(),
                    20);

                await Task.Delay(TimeSpan.FromSeconds(secondsToSleepBeforeIterations));
            }
        }

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            ConsoleColor.Cyan.WriteLine($"========= Blocks [{startBlock} , {endBlock}] =========");

            List<Task> tasks = new();

            for (ulong i = startBlock; i < endBlock; i++)
                tasks.Add(IndexBlock(i));

            await Task.WhenAll(tasks);

            if (BlockQueue.Count >= QueueSize)
            {
                await CommitIndexedBlocks();
            }
        }

        private async Task IndexBlock(ulong blockNubmer)
        {
            if (await this.ContainsBlock(new Block { BlockNumber = (long)blockNubmer }))
            {
                ConsoleColor.Yellow.WriteLine($"Block {blockNubmer} is already indexed. Skipping");
                return;
            }


            var currentBlock = new Block { BlockNumber = (long)blockNubmer, IndexingStatus = BlocksRepository.BlockStatus.INDEXING.ToString() };

            try
            {
                var txs = await GetBlockTransactions(blockNubmer);

                if (!txs.Any())
                {
                    ConsoleColor.Yellow.WriteLine($"no transactions in block {blockNubmer}. Skipping");
                }
                else
                {
                    foreach (var tx in txs)
                        await IndexTransaction(tx);
                }

                currentBlock.IndexingStatus = BlocksRepository.BlockStatus.INDEXED.ToString();
                currentBlock.Transactions = txs;
                BlockQueue.Enqueue(currentBlock);

                ConsoleColor.Green.WriteLine($"Block {blockNubmer} added to InsertBlockQueue");
            }
            catch (Exception ex)
            {
                currentBlock.IndexingStatus = BlocksRepository.BlockStatus.FAILED.ToString();
                currentBlock.Transactions = null;

                BlockQueue.Enqueue(currentBlock);

                ConsoleColor.Red.WriteLine($"GetBlockTransactions() Failed on block {blockNubmer}. Ex: {ex}");
            }
        }

        private async Task IndexTransaction(Transaction tx)
        {
            IEnumerable<Web3Tracer.Models.TraceResult> trace;

            try
            {
                trace = await _web3Tracer.GetTracesForTransaction(tx.TransactionHash);
            }
            catch (Exception ex)
            {
                ConsoleColor.Red.WriteLine($"Unnable to get trace of {tx.TransactionHash}. Skipping internal calls. Ex message: {ex.Message}");

                trace = null;
            }


            if (trace is null)
            {
                tx.Calls.Add(new Call()
                {
                    To = tx.RawTransaction.To,
                    MethodId = tx.RawTransaction.MethodId,
                    From = tx.RawTransaction.From
                });

                ConsoleColor.Magenta.WriteLine("Trace is null, no internal transactions recorded");
                return;
            }

            foreach (var t in trace)
                IndexCall(t, tx);

            ConsoleColor.Green.WriteLine(tx.TransactionHash);
        }

        private void IndexCall(Web3Tracer.Models.TraceResult trace, Transaction tx)
        {
            if (!ValidCallTypes.Contains(trace.CallType?.ToLower() ?? "")) // if somehow callType is null - null argument exception will not be thrown
            {
                ConsoleColor.DarkBlue.WriteLine($"Transactions with callType {trace.CallType} is not included");
                return;
            }

            ConsoleColor.DarkBlue.WriteLine($"\t{trace.CallType}");

            tx.Calls.Add(new Call
            {
                From = trace.From,
                To = trace.To,
                MethodId = _getMethodIdFromInput(trace.Input),
                TransactionHash = tx.TransactionHash,
                Type = trace.CallType,
                Time = trace.Time
            });
        }


        private async Task<bool> ContainsBlock(Block block)
        {
            using var bRepo = new BlocksRepository(_connectionString);
            return await bRepo.ContainsBlockAsync(block);
        }

        private async Task<IEnumerable<Transaction>> GetBlockTransactions(ulong blockNubmer)
        {
            var block = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNubmer.ToHexBigInteger());

            return block.Transactions.Select(t =>
            {
                string input = t.Value.HexValue;

                return new Transaction
                {
                    TransactionHash = t.TransactionHash,
                    Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.ToUlong()).UtcDateTime,
                    BlockId = (long)blockNubmer,
                    RawTransaction = new RawTransaction
                    {
                        From = t.From,
                        To = t.To, // todo: research {meaning of contractAddress; empty to address}
                        MethodId = _getMethodIdFromInput(input),
                        Value = t.Value?.ToString(),
                    }
                };
            });
        }

        private string _getMethodIdFromInput(string value)
        {
            if (value is null) return String.Empty;
            return value.Substring(0, value.Length > 10 ? 10 : value.Length);
        }


        public static List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        private readonly IWeb3Tracer _web3Tracer;

        private readonly string _connectionString;
    }
}