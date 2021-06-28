using Dapper;
using Database.Models;
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
               "delete from Blocks where BlockId=@BlockNumber";

            using (var tRepo = new TransactionRepository(SqlConnection.ConnectionString))
                await tRepo.RemoveBlockTransftions(block);

            await SqlConnection.ExecuteAsync(sql, block);
        }

        public async Task AddNewBlocksWithTransactionsAndCallsAsync(IEnumerable<Block> blocks)
        {
            string getRowStringForBlocks(Block b) => $"({b.BlockNumber})";
            string getRowStringForTx(Transaction tx) => $"(convert(binary(32),'{tx.TransactionHash}',2),{tx.BlockId},'{tx.TimeStamp}', convert(binary(32), '{tx.GasPrice}', 2))";
            string getRowStringForCall(Call c) => $"(convert(binary(32),'{c.TransactionHash}',2),{$"'{c.Error}'".Replace("''", "NULL")},'{(int)c.Type}',convert(binary(20),'{c.From}',2), convert(binary(20),'{c.To}',2) ,convert(binary(4),'{c.MethodId ?? ""}',2), convert(binary(32),'{c.GasUsed ?? ""}',2), convert(binary(32),'{c.GasSended ?? ""}',2), convert(binary(32),'{c.Value ?? ""}',2))";

            string valueNamesForBlocks = "[BlockNumber]";
            string valueNamesForTxs = "[TransactionHash],[BlockId],[TimeStamp],[GasPrice]";
            string valueNamesForCalls = "[TransactionHash],[Error],[Type],[From],[To],[MethodId],[GasUsed],[GasSended],[Value]";

            List<Transaction> txs = new List<Transaction>();
            List<Call> calls = new List<Call>();

            foreach (var block in blocks)
                txs = txs.Concat(block.Transactions).AsList();

            foreach (var tx in txs)
                calls = calls.Concat(tx.Calls).AsList();

            var insertValuesBlocks = string.Join(' ', blocks.Select(b => $"{getRowStringForBlocks(b)},"));
            var insertValuesTxs = string.Join(' ', txs.Select(t => $"{getRowStringForTx(t)},"));
            var insertValuesCalls = string.Join(' ', calls.Select(c => $"{getRowStringForCall(c)},"));

            var sql =
                ((((string.IsNullOrEmpty(insertValuesBlocks) ? "" : $"insert into Blocks({valueNamesForBlocks}) select {valueNamesForBlocks} from (values {insertValuesBlocks})sub ({valueNamesForBlocks});") +
                (string.IsNullOrEmpty(insertValuesTxs) ? "" : $"insert into Transactions({valueNamesForTxs}) select {valueNamesForTxs} from (values {insertValuesTxs})sub ({valueNamesForTxs});")).Replace("''", "NULL") +
                (string.IsNullOrEmpty(insertValuesCalls) ? "" : $"insert into Calls({valueNamesForCalls}) select {valueNamesForCalls} from (values {insertValuesCalls})sub ({valueNamesForCalls});")))
                    .Replace(",;", ";").Replace(",)", ")").Replace("'NULL'", "NULL").Replace("'null'", "NULL");

            await SqlConnection.ExecuteAsync(sql);
        }


        public async Task AddNewBlockAsync(Block block)
        {
            var sql = "insert into Blocks(BlockNumber) " +
                $"values (@BlockNumber)";

            await SqlConnection.ExecuteAsync(sql, block);
        }
    }
}
