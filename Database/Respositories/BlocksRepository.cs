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

            return await SqlConnection.QueryAsync<Block>(sql, new { Status = status});
        }

    }
}
