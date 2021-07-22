using Dapper;
using System.Threading.Tasks;

namespace Database.Respositories
{
    public class PeymentRepository : RepositoryBase
    {
        public PeymentRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<int> CheckQuantityArgumentFunctionRequests(string address)
        {
            var sql = "SELECT ArgumentFunctionRequests " +
                        "FROM Users " +
                      $"WHERE Address=convert(binary(20),'{address}',1);";

            return await SqlConnection.QuerySingleAsync<int>(sql);
        }

        public async Task SetArgumentFunctionRequests(string address, int quantity)
        {
            int currentQuantity = await CheckQuantityArgumentFunctionRequests(address);

            var sql = "UPDATE Users " +
                         "SET ArgumentFunctionRequests=@ArgumentFunctionRequests " +
                      $"WHERE Address=convert(binary(20),'{address}',1);";

            await SqlConnection.ExecuteAsync(sql, new { ArgumentFunctionRequests = currentQuantity + quantity });
        }
    }
}
