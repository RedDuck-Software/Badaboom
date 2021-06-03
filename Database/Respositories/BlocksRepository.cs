using Dapper;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Respositories
{
    public class BlocksRepository : RepositoryBase
    {
        public enum BlockStatus { FAILED, INDEXED, INDEXING }

        public BlocksRepository(string connectionString) : base(connectionString) { }
        public BlocksRepository() : base(ConnectionStrings.DefaultConnection) { }

        public async Task<bool> ContainsBlockAsync(Block block)
            => await SqlConnection.ExecuteScalarAsync<bool>("select count(1) from Blocks where BlockNumber=@BlockNumber", block);


        public async Task<Block> GetLastIndexedBlockAsync()
        {
            var sql = "SELECT TOP 1 * FROM Blocks WHERE IndexingStatus='INDEXED' ORDER BY BlockNumber DESC";

            var res = await SqlConnection.QueryAsync<Block>(sql);
            return res.Count() == 0 ? null : res.First();
        }


        public async Task<IEnumerable<Block>> GetAllFailedBlocksAsync()
        {
            var sql =
                "select * from Blocks WHERE IndexingStatus='FAILED'";

            return await SqlConnection.QueryAsync<Block>(sql);
        }


        public async Task<IEnumerable<Block>> GetAllSuccessfulBlocksAsync()
        {
            var sql =
                "select * from Blocks where IndexingStatus='INDEXED'";

            return await SqlConnection.QueryAsync<Block>(sql);
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
    }
}
