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
        public enum BlockStatus { FAILED, INDEXED, INDEXING }

        public BlocksRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> ContainsBlockAsync(Block block)
            => await SqlConnection.ExecuteScalarAsync<bool>("select count(1) from Blocks where BlockNumber=@BlockNumber", block);


        public async Task<Block> GetLastIndexedBlockAsync()
        {
            var sql = "SELECT TOP 1 * FROM Blocks WHERE IndexingStatus='INDEXED' ORDER BY BlockNumber DESC";

            var res = await SqlConnection.QueryAsync<Block>(sql);
            return res.Count() == 0 ? null : res.First();
        }

        public async Task<IEnumerable<Block>> GetAllSuccessfulBlocksAsync()
            => await this.GetAllBlocksWithStatus("INDEXED");

        public async Task<IEnumerable<Block>> GetAllFailedBlocksAsync()
            => await this.GetAllBlocksWithStatus("FAILED");

        public async Task<IEnumerable<Block>> GetAllPendingBlocksAsync()
            => await this.GetAllBlocksWithStatus("INDEXING");


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
            string getRowStringForBlocks(Block b) => $"({b.BlockNumber},'{b.IndexingStatus}')";
            string getRowStringForTx(Transaction tx) => $"('{tx.TransactionHash}',{tx.BlockId},'{tx.Time.ToString("s")}')";
            string getRowStringForCall(Call c) => $"('{c.TransactionHash}','{c.Error}','{c.Type}','{c.From}','{c.To}','{c.MethodId}')";

            string valueNamesForBlocks = "BlockNumber, IndexingStatus";
            string valueNamesForTxs = "[TransactionHash],[BlockId],[Time]";
            string valueNamesForCalls = "[TransactionHash],[Error],[Type],[From],[To],[MethodId]";

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
                ((string.IsNullOrEmpty(insertValuesBlocks) ? "" : $"insert into Blocks({valueNamesForBlocks}) select {valueNamesForBlocks} from (values {insertValuesBlocks})sub ({valueNamesForBlocks});") +
                (string.IsNullOrEmpty(insertValuesTxs) ? "" : $"insert into Transactions({valueNamesForTxs}) select {valueNamesForTxs} from (values {insertValuesTxs})sub ({valueNamesForTxs});") +
                (string.IsNullOrEmpty(insertValuesCalls) ? "" : $"insert into Calls({valueNamesForCalls}) select {valueNamesForCalls} from (values {insertValuesCalls})sub ({valueNamesForCalls});"))
                    .Replace(",;", ";").Replace(",)", ")").Replace("''", "NULL");

            await SqlConnection.ExecuteAsync(sql);
        }


        public async Task AddNewBlockAsync(Block block, BlockStatus status)
        {
            var sql = "insert into Blocks(BlockNumber,IndexingStatus) " +
                $"values (@BlockNumber,'{status}')";

            await SqlConnection.ExecuteAsync(sql, block);
        }

        public async Task ChangeBlockStatusTo(Block block, BlockStatus status)
        {
            var sql =
                $"update Blocks set IndexingStatus='{status}' where BlockNumber=@BlockNumber";
            await SqlConnection.ExecuteAsync(sql, block);
        }


        private async Task<IEnumerable<Block>> GetAllBlocksWithStatus(string status)
        {
            var sql =
               "select * from Blocks WHERE IndexingStatus=@Status";

            return await SqlConnection.QueryAsync<Block>(sql, new { Status = status });
        }

    }
}
