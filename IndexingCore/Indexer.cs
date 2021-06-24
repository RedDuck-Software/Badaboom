﻿using System;
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
using IndexingCore.RpcProviders;
using Nethereum.JsonRpc.Client;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Web3Tracer.Tracers.Geth;

namespace IndexerCore
{
    public class Indexer
    {
        public Web3 Web3 => _web3Tracer.Web3;

        public IReadOnlyCollection<string> ValidCallTypes { get; } = new string[]
                { "create", "call", "delegatecall", "staticcall" , "callcode" };

        public ConcurrentQueue<Block> BlockQueue { get; set; } = new ConcurrentQueue<Block>();

        public readonly int QueueSize;

        public const int MAX_INSERT_ROWS = 1000;

        public readonly ILogger Logger;


        public Indexer(IWeb3Tracer web3Tracer, ILogger logger, string dbConnectionString, GetBlockIOProvider rpcProvider, int queueSize = MAX_INSERT_ROWS)
        {
            _web3Tracer = web3Tracer;
            _connectionString = dbConnectionString;
            QueueSize = queueSize;
            _rpcProvider = rpcProvider;
            Logger = logger;
        }

        public async Task<ulong> GetLatestBlockNumber()
        {
            try
            {
                return (await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();
            }
            catch (Exception)
            {

                if (_rpcProvider.IsAllTokensUsed)
                {
                    _rpcProvider.Reset();
                    await Task.Delay(TimeSpan.FromHours(24));
                }

                _web3Tracer.ChangeWeb3Provider(_rpcProvider.GetNextRpcUrl());

                return await GetLatestBlockNumber();
            }
        }

        public async Task IndexFailedAndPendingBlocks()
        {
            using (var bRepo = new BlocksRepository(_connectionString))
            {
                var failedBlocks = await bRepo.GetAllFailedBlocksAsync();

                if (failedBlocks is null) failedBlocks = new List<Block>();

                var pendingBlocks = await bRepo.GetAllPendingBlocksAsync();

                if (pendingBlocks is null) pendingBlocks = new List<Block>();

                var blocks = failedBlocks.Concat(pendingBlocks);

                Logger.LogInformation($"Indexing {failedBlocks.Count()} failed blocks and {pendingBlocks.Count()} locked in pending blocks\n\n");


                foreach (var block in blocks)
                {
                    await bRepo.RemoveBlockAsync(block);
                    await IndexBlock((ulong)block.BlockNumber);
                }
            }

            Logger.LogInformation("Indexing completed");
        }

        public async Task IndexInRangeParallel(ulong startBlock, ulong endBlock, ulong step = 20)
        {
            if (startBlock >= endBlock) throw new ArgumentException("Start block cannot be bigger then end block");

            Logger.LogInformation($"Indexing proccess started. Step - 20\n");

            var movements = (endBlock - startBlock) / step;

            for (ulong i = 0, j = 0; j < movements; i += step, j++)
                await this.IndexInRange(i + startBlock, i + step + startBlock);

            var multRes = startBlock + step * movements;

            if (multRes < endBlock)
                await this.IndexInRange(multRes, endBlock);
        }

        public async Task CommitIndexedBlocks()
        {
            Logger.LogInformation($"Commiting queued blocks. Current queue size is: { BlockQueue.Count}");

            // split queue into chunks because of t-sql limitation in 1000 rows per one sql query
            IEnumerable<IEnumerable<Block>> blockQueueChunked = ChunkBy(BlockQueue.ToList(), MAX_INSERT_ROWS);

            using (var bRepo = new BlocksRepository(_connectionString))
            {
                using (var tRepo = new TransactionRepository(_connectionString))
                {
                    foreach (var chunk in blockQueueChunked)
                    {
                        await bRepo.AddNewBlocksWithTransactionsAndCallsAsync(chunk);
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
                    (ulong)lastBlockRecorded.BlockNumber,
                    await this.GetLatestBlockNumber(),
                    20);

                await CommitIndexedBlocks();

                await Task.Delay(TimeSpan.FromSeconds(secondsToSleepBeforeIterations));
            }
        }

        public async Task IndexInRange(ulong startBlock, ulong endBlock)
        {
            var queueSnapshot = BlockQueue.ToArray();

            try
            {
                Logger.LogInformation($"========= Blocks [{startBlock} , {endBlock}] =========");

                List<Task> tasks = new List<Task>();

                for (ulong i = startBlock; i < endBlock; i++)
                    tasks.Add(IndexBlock(i));

                await Task.WhenAll(tasks);
            }
            catch (Exception) // if exception wasn`t handled in IndexBlock method - this is a Rpc call Exception
            {
                Logger.LogCritical("Exception occured while sending RpcRequest. Changing api key");

                BlockQueue = new ConcurrentQueue<Block>(queueSnapshot);

                _web3Tracer.ChangeWeb3Provider(_rpcProvider.GetNextRpcUrl());

                if (_rpcProvider.IsAllTokensUsed)
                {
                    _rpcProvider.Reset();
                    await Task.Delay(TimeSpan.FromHours(24));
                }

                await IndexInRange(startBlock, endBlock);

                if (BlockQueue.Count >= QueueSize)
                    await CommitIndexedBlocks();
            }
        }

        private async Task IndexBlock(ulong blockNubmer)
        {
            if (await this.ContainsBlock(new Block { BlockNumber = (long)blockNubmer }))
            {
                Logger.LogWarning($"Block {blockNubmer} is already indexed. Skipping");
                return;
            }

            var currentBlock = new Block { BlockNumber = (long)blockNubmer };

            try
            {
                var txs = (await GetBlockTransactions(blockNubmer)).ToList();

                if (!txs.Any())
                {
                    Logger.LogWarning($"no transactions in block {blockNubmer}. Skipping");
                }
                else
                {
                    foreach (var tx in txs)
                        await IndexTransaction(tx);
                }

                currentBlock.Transactions = txs.ToList();
                BlockQueue.Enqueue(currentBlock);

                Logger.LogInformation($"Block {blockNubmer} added to InsertBlockQueue");
            }
            catch (Exception ex)
            {
                if (ex is RpcResponseException || ex is RpcClientTimeoutException || ex is HttpRequestException) throw;

                Logger.LogCritical($"GetBlockTransactions() Failed on block {blockNubmer}. Ex: {ex}");
            }
        }

        private async Task<Transaction> IndexTransaction(Transaction tx)
        {
            IEnumerable<Web3Tracer.Models.TraceResult> trace;

            try
            {
                trace = await _web3Tracer.GetTracesForTransaction(tx.TransactionHash);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Unnable to get trace of {tx.TransactionHash}. Skipping internal calls. Ex message: {ex.Message}");

                trace = null;
            }


            if (trace is null)
            {
                tx.Calls.Add(new Call()
                {
                    TransactionHash = tx.TransactionHash,
                    To = tx.RawTransaction.To,
                    MethodId = tx.RawTransaction.MethodId,
                    From = tx.RawTransaction.From
                });

                Logger.LogWarning("Trace is null, no internal transactions recorded");
                return tx;
            }

            foreach (var t in trace)
                IndexCall(t, tx);

            Logger.LogInformation(tx.TransactionHash);
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
                MethodId = GetMethodIdFromInput(trace.Input).RemoveHashPrefix(),
                TransactionHash = tx.TransactionHash,
                Type = Call.CreataCallTypeFromString(trace.CallType)
            }); ;
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
                    TransactionHash = t.TransactionHash.RemoveHashPrefix(),
                    TimeStamp = (int)block.Timestamp.ToUlong(),
                    BlockId = (long)blockNubmer,
                    Calls = new List<Call>(),
                    RawTransaction = new RawTransaction
                    {   
                        From = t.From.RemoveHashPrefix(),
                        To = t.To.RemoveHashPrefix(),
                        MethodId = GetMethodIdFromInput(input).RemoveHashPrefix(),
                        Value = t.Value?.ToString(),
                    }
                };
            });
        }

        private string GetMethodIdFromInput(string value)
        {
            if (value is null) return String.Empty;
            return value.Substring(0, value.Length > 10 ? 10 : value.Length);
        }


        private static List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        private readonly GetBlockIOProvider _rpcProvider;

        private readonly IWeb3Tracer _web3Tracer;

        private readonly string _connectionString;
    }

    internal static class StringExtensions
    {
        public static string RemoveHashPrefix(this string hash)
           => hash.Length < 2 ?
                   hash.Substring(0, 2) == "0x" ?
                       hash.Remove(0, 2) : hash
                   : hash;
    }
}

