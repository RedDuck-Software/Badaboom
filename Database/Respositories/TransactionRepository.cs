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
                "select t.TransactionId, t.Hash, t.Time, c.CallId,c.TransactionId, c.ContractAddress, c.MethodId  from Transactions t" +
                "Inner join Calls c on c.TransactionId = t.TransactionId" +
                "where c.ContractAddress = @ContractAddress and " +
                "c.MethodId = @MethodId";


            return await SqlConnection.QueryAsync<Transaction>(sql, new { ContractAddress = address, MethodId = methodId });
        }


        public async Task<Transaction> AddNewTransactionAsync(Transaction tx)
        {
            var sql = "insert into Transactions(Hash,Time)" +
                $"values (@Hash,@Time) SELECT CAST(SCOPE_IDENTITY() AS INT)  ";

            try
            {
                var id = await SqlConnection.QueryAsync<int>(sql, tx);

                tx.Id = id.Single();

                return tx;
            }
            catch (SqlException)
            {
                return null;
            }
        }


        public async Task AddNewCallAsync(Call call)
        {
            var sql = "insert into Calls(TransactionId,[Error],[Type],[From],ContractAddress,[Value],MethodId)" +
                $"values(@TransactionId,@Error,@Type,@From,@ContractAddress,@Value,@MethodId)";

            try
            {
                await SqlConnection.ExecuteAsync(sql, call);
            }
            catch (SqlException ex) 
            {

            }
        }


        public async Task<IEnumerable<Call>> GetCallsByAddressAndMethodAsync(string address, string methodId)
        {
            var sql =
                "select * from Calls " +
                "where ContractAddress = @ContractAddress and " +
                "MethodId = @MethodId";

            return await SqlConnection.QueryAsync<Call>(sql, new { ContractAddress = address, MethodId = methodId });
        }
    }
}
