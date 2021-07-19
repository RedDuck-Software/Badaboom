using Database.Models;
using Database.Respositories;
using IndexingCore.RpcProviders;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web3Tracer.Tracers;

namespace IndexerCore
{
    public class Indexer
    {
        public Web3 Web3 => _web3Tracer.Web3;

        public IReadOnlyCollection<string> ValidCallTypes { get; }

        public ConcurrentQueue<Block> BlockQueue { get; set; } = new ConcurrentQueue<Block>();

        public readonly int QueueSize;

        public readonly ILogger Logger;

        public IEnumerable<string> AddressesToIndex { get; set; }


        public Indexer(IWeb3Tracer web3Tracer, ILogger logger, string dbConnectionString, InfuraProvider rpcProvider, int queueSize, IEnumerable<string> addressesToIndex = null)
        {
            _web3Tracer = web3Tracer;
            _connectionString = dbConnectionString;
            QueueSize = queueSize;
            _rpcProvider = rpcProvider;
            Logger = logger;
            AddressesToIndex = addressesToIndex == null || addressesToIndex.Count() == 0 ? null : addressesToIndex.Select(v => v.FormatHex());
            ValidCallTypes = Enum.GetNames(typeof(CallTypes)).Select(v => v.ToLower()).Where(v => v != CallTypes.NO_CALL_TYPE.ToString().ToLower()).ToArray();
        }

        public async Task<ulong> GetLatestBlockNumber()
        {
            try
            {
                return (await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();
            }
            catch (Exception)
            {
                _web3Tracer.ChangeWeb3Provider(_rpcProvider.GetNextRpcUrl());

                if (_rpcProvider.IsAllTokensUsed)
                {
                    Logger.LogError("All api keys are already used. Sleep for 24h to make them available again");
                    _rpcProvider.Reset();
                }

                return await GetLatestBlockNumber();
            }
        }


        public async Task IndexInRangeParallel(long startBlock, long endBlock, long step = 20)
        {
            if (startBlock > endBlock) step *= -1;

            Logger.LogInformation($"Indexing proccess started. Step - 20\n");

            var movements = Math.Abs((endBlock - startBlock) / step);

            for (long i = 0, j = 0; j < movements; i += step, j++)
                await this.IndexInRange((ulong)(i + startBlock), (ulong)(i + step + startBlock));

            var multRes = startBlock + step * movements;

            if (startBlock < endBlock && multRes < endBlock)
                await this.IndexInRange((ulong)multRes, (ulong)endBlock);

            if (startBlock > endBlock && multRes > startBlock)
                await this.IndexInRange((ulong)multRes, (ulong)endBlock);

            if (!BlockQueue.IsEmpty)
                await this.CommitIndexedBlocks();
        }

        public async Task CommitIndexedBlocks()
        {
            Logger.LogInformation($"Commiting queued blocks. Current queue size is: { BlockQueue.Count}");

            var blockQueueList = BlockQueue.ToList();

            using (var bRepo = new BlocksRepository(_connectionString))
            {
                try
                {
                    await bRepo.AddNewBlocksWithTransactionsAndCallsAsync(blockQueueList);

                    Logger.LogInformation("Successfully saved");
                }
                catch (Exception ex)
                {
                    Logger.LogCritical("Error while saving BlockQueue into database. ex: " + ex.Message);

                    foreach (var block in blockQueueList)
                    {
                        try
                        {
                            await bRepo.RemoveBlockAsync(block);
                        }
                        catch (Exception e)
                        {
                            Logger.LogCritical($"Error while removing block {block.BlockNumber}. ex: " + e.Message);
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
                    lastBlockRecorded = await bRepo.GetLastIndexedBlockAsync() ?? new Block() { BlockNumber = 0 };

                await this.IndexInRangeParallel(
                    lastBlockRecorded.BlockNumber,
                    (long)await this.GetLatestBlockNumber(),
                    20);

                await CommitIndexedBlocks();

                await Task.Delay(TimeSpan.FromSeconds(secondsToSleepBeforeIterations));
            }
        }

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            var queueSnapshot = BlockQueue.ToArray();

            if (startBlock > endBlock)
            {
                var c = startBlock;
                startBlock = endBlock;
                endBlock = c;
            }

            try
            {
                Logger.LogInformation($"========= Blocks [{startBlock} , {endBlock}] =========");

                List<Task> tasks = new List<Task>();

                for (ulong i = startBlock; i < endBlock; i++)
                    tasks.Add(IndexBlock(i));

                await Task.WhenAll(tasks);

                if (BlockQueue.Count >= QueueSize)
                    await CommitIndexedBlocks();
            }
            catch (Exception ex) // if exception wasn`t handled in IndexBlock method - this is a Rpc call Exception
            {
                Logger.LogCritical($"Super critical exception occured. Exiting the app. Ex: {ex.Message}");

                System.Environment.Exit(1);

                /*BlockQueue.Clear();

                _web3Tracer.ChangeWeb3Provider(_rpcProvider.GetNextRpcUrl());

                if (_rpcProvider.IsAllTokensUsed)
                    _rpcProvider.Reset();

                await IndexInRange(startBlock, endBlock);*/
            }
        }

        private async Task IndexBlock(ulong blockNubmer)
        {
            var currentBlock = new Block { BlockNumber = (int)blockNubmer };

            try
            {
                var txs = (await GetBlockTransactions(blockNubmer)).ToList().Where(v => AddressesToIndex == null ? true :
                            AddressesToIndex.Contains(v.RawTransaction.To ?? "")
                            || AddressesToIndex.Contains(v.RawTransaction.From ?? "")).ToList();

                if (!txs.Any())
                {
                    Logger.LogWarning($"no transactions in block {blockNubmer}. Skipping");
                }
                else
                {
                    foreach (var tx in txs)
                    {
                        if (!await ContainsTransactions(tx))
                        {
                            if ()
                            {

                            }
                            await IndexTransaction(tx);
                        }
                        else
                        {
                            Logger.LogWarning($"Tx with 0x{tx.TransactionHash} is already indexed");
                            txs.Remove(tx);
                        }
                    }
                }

                currentBlock.Transactions = txs.ToList();
                BlockQueue.Enqueue(currentBlock);

                Logger.LogInformation($"Block {blockNubmer} added to InsertBlockQueue");
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetBlockTransactions() Failed on block {blockNubmer}. Ex: {ex.Message}");
                throw ex;
            }
        }

        private async Task<Transaction> IndexTransaction(Transaction tx, bool indexInnerCalls = false)
        {
            IEnumerable<Web3Tracer.Models.TraceResult> trace = null;

            if (indexInnerCalls)
            {
                try
                {
                    trace = await _web3Tracer.GetTracesForTransaction("0x" + tx.TransactionHash);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Unnable to get trace of {tx.TransactionHash}. Skipping internal calls. Ex message: {ex.Message}");

                    trace = null;
                }
            }

            if (trace is null || indexInnerCalls == false)
            {
                tx.Calls.Add(new Call()
                {
                    TransactionHash = tx.TransactionHash,
                    To = tx.RawTransaction.To,
                    MethodId = tx.RawTransaction.MethodId,
                    From = tx.RawTransaction.From,
                    Input = tx.RawTransaction.Input,
                    Type = CallTypes.Call
                });

                if (trace is null)
                    Logger.LogWarning("Trace is null, no internal transactions recorded");

                return tx;
            }

            if (indexInnerCalls)
                foreach (var t in trace)
                    IndexCall(t, tx);

            Logger.LogInformation(tx.TransactionHash);

            tx.TransactionHash = tx.TransactionHash.RemoveHashPrefix();
            return tx;
        }

        private void IndexCall(Web3Tracer.Models.TraceResult trace, Transaction tx)
        {
            if (!ValidCallTypes.Contains(trace.CallType?.ToLower() ?? "")) // if somehow callType is null - null argument exception will not be thrown
            {
                Logger.LogWarning($"Transactions with callType {trace.CallType} is not included");
                return;
            }

            Logger.LogDebug($"\t{trace.CallType}");

            tx.Calls.Add(new Call
            {
                From = trace.From.RemoveHashPrefix(),
                To = trace.To.RemoveHashPrefix(),
                MethodId = GetMethodIdFromInput(trace.Input)?.FormatHex(),
                TransactionHash = tx.TransactionHash,
                Type = Enum.Parse<CallTypes>(trace.CallType, true),
                Error = trace.Error != null && trace.Error.Length > 50 ? trace.Error.Substring(0, 50) : trace.Error,
            });
        }


        private async Task<bool> ContainsBlock(Block block)
        {
            using var bRepo = new BlocksRepository(_connectionString);
            return await bRepo.ContainsBlockAsync(block);
        }

        private async Task<bool> ContainsTransactions(Transaction tx)
        {
            using var tRepo = new TransactionRepository(_connectionString);
            return await tRepo.ContainsTransactionAsync(tx);
        }

        private async Task<IEnumerable<Transaction>> GetBlockTransactions(ulong blockNubmer)
        {
            var block = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNubmer.ToHexBigInteger());

            return block.Transactions.Select(t =>
            {
                return new Transaction
                {
                    TransactionHash = t.TransactionHash.RemoveHashPrefix(),
                    TimeStamp = (int)block.Timestamp.ToUlong(),
                    BlockId = (int)blockNubmer,
                    Calls = new List<Call>(),
                    GasPrice = t.Gas?.HexValue?.FormatHex(),
                    RawTransaction = new RawTransaction
                    {
                        From = t.From.RemoveHashPrefix(),
                        To = t.To.RemoveHashPrefix(),
                        MethodId = GetMethodIdFromInput(t.Input)?.FormatHex() ?? "",
                        Value = t.Value?.HexValue?.FormatHex(),
                        GasPrice = t.GasPrice?.HexValue?.FormatHex(),
                        Gas = t.Gas?.HexValue?.FormatHex(),
                        Input = t.Input?.FormatHex(),
                    }
                };
            });
        }

        private string GetMethodIdFromInput(string value)
        {
            if (string.IsNullOrEmpty(value)) return String.Empty;
            return value.Substring(0, value.Length > 10 ? 10 : value.Length);
        }

        private readonly InfuraProvider _rpcProvider;

        private readonly IWeb3Tracer _web3Tracer;

        private readonly string _connectionString;
    }

    internal static class StringExtensions
    {
        public static string FormatHex(this string h)
        {
            var hash = h.RemoveHashPrefix().ZeroHashFormatting();

            return string.IsNullOrEmpty(hash) ? hash :
                hash.Length % 2 == 0 ? hash :
                    hash + "0";
        }


        public static string ZeroHashFormatting(this string hash) => hash == "0" ? null : hash;

        public static string RemoveHashPrefix(this string hash)
           => string.IsNullOrEmpty(hash) ? hash :
                hash.Length >= 2 ?
                   hash.Substring(0, 2) == "0x" ?
                       hash.Remove(0, 2) : hash
                   : hash;
    }
}

