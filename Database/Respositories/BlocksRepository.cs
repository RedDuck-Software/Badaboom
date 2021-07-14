using Dapper;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Respositories
{
    public class BlocksRepository : RepositoryBase
    {
        public BlocksRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> ContainsBlockAsync(Block block)
            => await SqlConnection.ExecuteScalarAsync<bool>("select count(1) from Blocks where BlockNumber=@BlockNumber", block);


        public async Task<Block> GetLastIndexedBlockAsync()
        {
            var sql = "SELECT TOP 1 * FROM Blocks ORDER BY BlockNumber DESC";

            var res = await SqlConnection.QueryAsync<Block>(sql);
            return res.Count() == 0 ? null : res.First();
        }

        public async Task RemoveBlockAsync(Block block)
        {
            var sql =
               "delete from Blocks where BlockNumber=@BlockNumber";

            using (var tRepo = new TransactionRepository(SqlConnection.ConnectionString))
                await tRepo.RemoveBlockTransftions(block);

            await SqlConnection.ExecuteAsync(sql, block);
        }

        public async Task AddNewBlocksWithTransactionsAndCallsAsync(IEnumerable<Block> blocks)
        {
            string getRowStringForBlocks(Block b) => $"{b.BlockNumber}";
            string getRowStringForTx(Transaction tx) => $"convert(binary(32),'{tx.TransactionHash}',2),{tx.BlockId},'{tx.TimeStamp}', convert(binary(8), '{tx.GasPrice}', 2)";
            string getRowStringForCall(Call c) => $"convert(binary(32),'{c.TransactionHash}',2),{$"'{c.Error}'".Replace("''", "NULL")},'{(int)c.Type}',convert(binary(20),'{c.From}',2), convert(binary(20),'{c.To}',2) ,convert(binary(4),'{c.MethodId ?? ""}',2), convert(varbinary(MAX),'{c.Input ?? ""}',2)";

            string getInsertBlocksSql(string valueNames, string values) => $"insert into Blocks({valueNames}) values({values});";
            string getInsertTransactionSql(string valueNames, string values) => $"insert into Transactions({valueNames}) values({values});";
            string getInsertCallSql(string valueNames, string values) => $"insert into Calls({valueNames}) values({values});";

            string valueNamesForBlocks = "[BlockNumber]";
            string valueNamesForTxs = "[TransactionHash],[BlockId],[TimeStamp],[GasPrice]";
            string valueNamesForCalls = "[TransactionHash],[Error],[Type],[From],[To],[MethodId],[Input]";

            List<Transaction> txs = new List<Transaction>();
            List<Call> calls = new List<Call>();

            foreach (var block in blocks)
                txs = txs.Concat(block.Transactions).AsList();

            foreach (var tx in txs)
                calls = calls.Concat(tx.Calls).AsList();

            var insertValuesBlocks = string.Join(' ', blocks.Select(b => getInsertBlocksSql(valueNamesForBlocks, getRowStringForBlocks(b)))).Replace("''", "NULL");
            var insertValuesTxs = string.Join(' ', txs.Select(t => getInsertTransactionSql(valueNamesForTxs, getRowStringForTx(t)))).Replace("''", "NULL");
            var insertValuesCalls = string.Join(' ', calls.Select(c => getInsertCallSql(valueNamesForCalls, getRowStringForCall(c))));

            var sql =
                (string.IsNullOrEmpty(insertValuesBlocks)? "" : insertValuesBlocks) +
                (string.IsNullOrEmpty(insertValuesTxs) ? "" : insertValuesTxs) +
                (string.IsNullOrEmpty(insertValuesCalls) ? "" : insertValuesCalls)
                    .Replace(",;", ";").Replace(",)", ")").Replace("'NULL'", "NULL").Replace("'null'", "NULL");

            await SqlConnection.ExecuteAsync(sql, commandTimeout: TimeSpan.FromHours(2).Seconds);
        }


        public async Task AddNewBlockAsync(Block block)
        {
            var sql = "insert into Blocks(BlockNumber) " +
                $"values (@BlockNumber)";

            await SqlConnection.ExecuteAsync(sql, block);
        }
    }
}
