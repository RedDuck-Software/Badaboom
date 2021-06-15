﻿using Dapper;
using Database.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Respositories
{
    public class TransactionRepository : RepositoryBase
    {
        public TransactionRepository(string connectionString) : base(connectionString) { }


        public async Task<IEnumerable<Transaction>> GetTransactionsByAddressAndMethod(string address, string methodId)
        {
            var sql =
                "select t.TransactionId, t.Hash, t.Time, c.CallId,c.TransactionId, c.ContractAddress, c.MethodId  from Transactions t " +
                "Inner join Calls c on c.TransactionId=t.TransactionId " +
                "where c.ContractAddress = @ContractAddress and " +
                "c.MethodId = @MethodId ";


            return await SqlConnection.QueryAsync<Transaction>(sql, new { ContractAddress = address, MethodId = methodId });
        }


        public async Task RemoveBlockTransftions(Block block)
        {
            var blockTransactions = await this.GetBlockTransactions(block);

            if (blockTransactions is null || !blockTransactions.Any()) return;

            string sql;

            foreach (var tx in blockTransactions)
            {
                sql =
                    "delete from [Calls] " +
                    "where [TransactionId]=@Id";

                await SqlConnection.ExecuteAsync(sql, tx);



                sql =
                    "delete from [Transactions] " +
                    "where [TransactionId]=@Id";

                await SqlConnection.ExecuteAsync(sql, tx);
            }
        }


        public async Task<IEnumerable<Transaction>> GetBlockTransactions(Block block)
        {
            var sql =
                "select * from [Transactions] " +
                "where BlockId=@BlockNumber";

            return await SqlConnection.QueryAsync<Transaction>(sql, block);
        }


        public async Task<Transaction> AddNewTransactionAsync(Transaction tx)
        {
            var sql = "insert into Transactions(BlockId,Hash,Time) " +
                $"values (@BlockId,@Hash,@Time) SELECT CAST(SCOPE_IDENTITY() AS INT)";


            var id = await SqlConnection.QueryAsync<int>(sql, tx);

            tx.Id = id.Single();

            return tx;
        }


        public async Task AddNewCallAsync(Call call)
        {
            var sql = "insert into Calls(TransactionId,[Error],[Type],[From],ContractAddress,[Value],MethodId) " +
                $"values(@TransactionId,@Error,@Type,@From,@ContractAddress,@Value,@MethodId)";


            await SqlConnection.ExecuteAsync(sql, call);

        }


        public async Task<IEnumerable<Call>> GetCallsByAddressAndMethodAsync(string address, string methodId)
        {
            var sql =
                "select * from Calls " +
                "where ContractAddress = @ContractAddress and " +
                "MethodId = @MethodId";

            return await SqlConnection.QueryAsync<Call>(sql, new { ContractAddress = address, MethodId = methodId });
        }
    }
}
