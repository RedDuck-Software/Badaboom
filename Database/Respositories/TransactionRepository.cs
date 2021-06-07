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

        public TransactionRepository() : base(ConnectionStrings.DefaultConnection) { }


        public async Task<IEnumerable<Transaction>> GetTransactionsByAddressAndMethod(string address, string methodId)
        {
            var sql =
                "select t.TransactionId, t.Hash, t.Time, c.CallId,c.TransactionId, c.To, c.MethodId  from Transactions t" +
                "Inner join Calls c on c.TransactionId = t.TransactionId" +
                "where c.To = @To and " +
                "c.MethodId = @MethodId";


            return await SqlConnection.QueryAsync<Transaction>(sql, new { To = address, MethodId = methodId });
        }


        public async Task<Transaction> AddNewTransactionAsync(Transaction tx)
        {
            var sql = "insert into Transactions(BlockId,Hash,Time)" +
                $"values (@BlockId,@Hash,@Time) SELECT CAST(SCOPE_IDENTITY() AS INT)";


            var id = await SqlConnection.QueryAsync<int>(sql, tx);

            tx.Id = id.Single();

            return tx;
        }


        public async Task AddNewCallAsync(Call call)
        {
            var sql = "insert into Calls(TransactionId,[Error],[Type],[From],[To],[Value],[MethodId])" +
                $"values(@TransactionId,@Error,@Type,@From,@To,@Value,@MethodId)";


            await SqlConnection.ExecuteAsync(sql, call);

        }


        public async Task<IEnumerable<Call>> GetCallsByAddressAndMethodAsync(string address, string methodId)
        {
            var sql =
                "select * from Calls " +
                "where ContractAddress = @To and " +
                "MethodId = @MethodId";

            return await SqlConnection.QueryAsync<Call>(sql, new { To = address, MethodId = methodId });
        }
    }
}
