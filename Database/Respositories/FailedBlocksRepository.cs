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
    public class FailedBlocksRepository : RepositoryBase
    {
        public FailedBlocksRepository(string connectionString) : base(connectionString) { }
        public FailedBlocksRepository() : base(ConnectionStrings.DefaultConnection) { }


        public async Task<IEnumerable<FailedBlock>> GetAllFailedBlocksAsync()
        {
            var sql =
                "select * from FailedBlocks";

            return await SqlConnection.QueryAsync<FailedBlock>(sql);
        }


        public async Task AddNewFailedBlockAsync(FailedBlock block)
        {
            var sql = "insert into FailedBlocks(BlockNumber) " +
                "values (@BlockNumber)";

            try
            {
                await SqlConnection.ExecuteAsync(sql, block);
            }
            catch (SqlException){}
        }

        public async Task DeleteFailedBlockAsync(FailedBlock block)
        {
            var sql = "delete from FailedBlocks " +
                "where BlockNumber = @BlockNumber";

            try
            {
                await SqlConnection.ExecuteAsync(sql, block);
            }
            catch (SqlException) { }
        }
    }
}
