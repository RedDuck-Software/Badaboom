using Dapper;
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


        public async Task RemoveBlockTransftions(Block block)
        {
            var blockTransactions = await this.GetBlockTransactions(block);

            if (blockTransactions is null || !blockTransactions.Any()) return;

            string sql;

            foreach (var tx in blockTransactions)
            {
                sql =
                    "delete from [Calls] " +
                    "where [TransactionHash]=@TransactionHash";

                await SqlConnection.ExecuteAsync(sql, tx);



                sql =
                    "delete from [Transactions] " +
                    "where [TransactionHash]=@TransactionHash";

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

        public async Task AddNewCallAsync(Call call)
        {
            var sql = "insert into Calls(TransactionId,[Error],[Type],[From],[To],MethodId) " +
                $"values(@TransactionHash,@Error,{(int)call.Type},@From,@To,@MethodId)";

            await SqlConnection.ExecuteAsync(sql, call);

        }

        public async Task<IEnumerable<Call>> GetCallsByAddressAndMethodAsync(string address, string methodId)
        {
            var sql =
                "select * from Calls " +
                "where To = @To and " +
                "MethodId = @MethodId";

            return await SqlConnection.QueryAsync<Call>(sql, new { ContractAddress = address, MethodId = methodId });
        }
    }
}
