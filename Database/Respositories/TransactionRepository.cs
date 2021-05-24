﻿using Dapper;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Respositories
{
    public class TransactionRepository : RepositoryBase
    {
        public TransactionRepository(string connectionString) : base(connectionString) {}
        public TransactionRepository() : base(ConnectionStrings.DefaultConnection) {}


        public async Task<IEnumerable<Transaction>> GetTransactionsByAddressAndMethod(string address, string methodId)
        {
            var sql =
                "select * from Transactions " +
                "where ContractAddress = @ContractAddress and " +
                "MethodId = @MethodId";

            return await SqlConnection.QueryAsync<Transaction>(sql, new { ContractAddress = address, MethodId = methodId });
        }


        public async Task AddNewTransactionAsync(Transaction tx)
        {
            var sql = "insert into Transactions(ContractAddress,Hash,MethodId,Time) " +
                "values (@ContractAddress, @Hash, @MethodId, @Time)";

            try
            {
                await SqlConnection.ExecuteAsync(sql, tx);
            }
            catch (SqlException) { }
        }

    }
}
