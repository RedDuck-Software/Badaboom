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

namespace IndexerCore
{
    public class Indexer
    {
        public Web3 Web3 => _web3Tracer.Web3;

        public IReadOnlyCollection<string> ValidCallTypes { get; } = new string[]
                { "create", "call", "delegatecall", "staticcall" , "callcode" };

        public Indexer(IWeb3Tracer web3Tracer, string dbConnectionString)
        {
            _web3Tracer = web3Tracer;
            _connectionString = dbConnectionString;
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
        }

        private async Task IndexBlock(ulong blockNubmer)
        {
            if (await this.ContainsBlock(new Block { BlockNumber = (long)blockNubmer }))
            {
                ConsoleColor.Yellow.WriteLine($"Block {blockNubmer} is already indexed. Skipping");
                return;
            }


            try
            {
                using (var bRepo = new BlocksRepository(_connectionString))
                    await bRepo.AddNewBlockAsync(new Block { BlockNumber = (long)blockNubmer }, BlocksRepository.BlockStatus.INDEXING);


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

                using (var bRepo = new BlocksRepository(_connectionString))
                    await bRepo.ChangeBlockStatusTo(new Block { BlockNumber = (long)blockNubmer }, BlocksRepository.BlockStatus.INDEXED);

                ConsoleColor.Green.WriteLine($"Block {blockNubmer} indexed");
            }
            catch (Exception ex)
            {
                var block = new Block { BlockNumber = (long)blockNubmer };

                using (var repo = new BlocksRepository(_connectionString))
                {

                    if (await ContainsBlock(block))
                        await repo.ChangeBlockStatusTo(block, BlocksRepository.BlockStatus.FAILED);
                    else
                        await repo.AddNewBlockAsync(block, BlocksRepository.BlockStatus.FAILED);
                }

                using (var repo = new TransactionRepository(_connectionString))
                    await repo.RemoveBlockTransftions(block);


                ConsoleColor.Red.WriteLine($"GetBlockTransactions() Failed on block {blockNubmer}. Ex: {ex}");
            }
        }

        private async Task IndexTransaction(Transaction tx)
        {
            Transaction txInserted;

            using (var repo = new TransactionRepository(_connectionString))
                txInserted = await repo.AddNewTransactionAsync(tx);

            IEnumerable<Web3Tracer.Models.TraceResult> trace;

            try
            {
                trace = await _web3Tracer.GetTracesForTransaction(tx.Hash);
            }
            catch (Exception ex)
            {
                ConsoleColor.Red.WriteLine($"Unnable to get trace of {tx.Hash}. Skipping internal calls. Ex message: {ex.Message}");

                trace = null;
            }


            if (trace is null)
            {
                await this.AddNewCallAsync(
                    new Call()
                    {
                        TransactionId = txInserted.Id,
                        To = tx.RawTransaction.To,
                        MethodId = tx.RawTransaction.MethodId,
                        From = tx.RawTransaction.From
                    });

                return;
            }

            foreach (var t in trace)
                await IndexCall(t, txInserted);

            ConsoleColor.Green.WriteLine(tx.Hash);
        }

        private async Task IndexCall(Web3Tracer.Models.TraceResult trace, Transaction tx)
        {
            if (!ValidCallTypes.Contains(trace.CallType?.ToLower() ?? "")) // if somehow callType is null - null argument exception will not be thrown
            {
                ConsoleColor.DarkBlue.WriteLine($"Transactions with callType {trace.CallType} is not included");
                return;
            }

            ConsoleColor.DarkBlue.WriteLine($"\t{trace.CallType}");


            await this.AddNewCallAsync(
                new Call
                {
                    From = trace.From,
                    To = trace.To,
                    MethodId = _getMethodIdFromInput(trace.Input),
                    TransactionId = tx.Id,
                    Type = trace.CallType,
                    Time = trace.Time
                });
        }


        private async Task<bool> ContainsBlock(Block block)
        {
            using var bRepo = new BlocksRepository(_connectionString);
            return await bRepo.ContainsBlockAsync(block);
        }

        private async Task AddNewCallAsync(Call call)
        {
            using var cRepo = new TransactionRepository(_connectionString);
            await cRepo.AddNewCallAsync(call);
        }

        private async Task<IEnumerable<Transaction>> GetBlockTransactions(ulong blockNubmer)
        {
            var block = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNubmer.ToHexBigInteger());

            return block.Transactions.Select(t =>
            {
                string input = t.Value.HexValue;

                return new Transaction
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.ToUlong()).UtcDateTime,
                    Hash = t.TransactionHash,
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

        private readonly IWeb3Tracer _web3Tracer;

        private readonly string _connectionString;
    }
}