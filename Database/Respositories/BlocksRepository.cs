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
        public BlocksRepository(string connectionString) : base(connectionString) { }
        public BlocksRepository() : base(ConnectionStrings.DefaultConnection) { }




        public async Task<bool> ContainsSuccessBlockAsync(Block block)
            => await SqlConnection.ExecuteScalarAsync<bool>("select count(1) from SuccessfulBlocks where BlockNumber=@BlockNumber", block);


        public async Task<Block> GetLastSuccessfulBlockAsync()
        {
            var sql = "SELECT TOP 1 * FROM SuccessfulBlocks ORDER BY BlockNumber DESC";

            var res = await SqlConnection.QueryAsync<Block>(sql);
            return res.Count() == 0 ? null : res.First();
        }


        public async Task<IEnumerable<Block>> GetAllFailedBlocksAsync()
        {
            var sql =
                "select * from FailedBlocks";

            return await SqlConnection.QueryAsync<Block>(sql);
        }


        public async Task AddNewFailedBlockAsync(Block block)
        {
            var sql = "insert into FailedBlocks(BlockNumber) " +
                "values (@BlockNumber)";


            await SqlConnection.ExecuteAsync(sql, block);
        }


        public async Task AddNewSuccessBlockAsync(Block block)
        {
            var sql = "insert into SuccessfulBlocks(BlockNumber) " +
                "values (@BlockNumber)";

            await SqlConnection.ExecuteAsync(sql, block);
        }

        public async Task<IEnumerable<Block>> GetAllSuccessfulBlocksAsync()
        {
            var sql =
                "select * from SuccessfulBlocks";

            return await SqlConnection.QueryAsync<Block>(sql);
        }


        public async Task DeleteFailedBlockAsync(Block block)
        {
            var sql = "delete from FailedBlocks " +
                "where BlockNumber = @BlockNumber";

            await SqlConnection.ExecuteAsync(sql, block);
        }

    }
}
