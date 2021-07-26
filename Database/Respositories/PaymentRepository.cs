using Dapper;
using System.Threading.Tasks;
using System;

namespace Database.Respositories
{
    public class PeymentRepository : RepositoryBase
    {
        public PeymentRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<int?> CheckQuantityArgumentFunctionRequests(string address, string productType)
        {
            var sql = "SELECT Quantity " +
                        "FROM Users u " +
                  "INNER JOIN UsersProducts up " +
                          "ON u.UserId = up.UserId " +
                  "INNER JOIN Products p " +
                          "ON up.ProductId = p.Id " +
                      $"WHERE u.Address = convert(binary(20), '{address}', 1) " +
                        $"AND p.ApiEndpoint = '{productType}';";

            return await SqlConnection.QuerySingleOrDefaultAsync<int?>(sql);
        }

        public async Task UpdateProductQuantity(string address, string productType, int totalQuantity)
        {

            var sql = "UPDATE UsersProducts " +
                         "SET Quantity = @Quantity " +
                        "FROM Users u " +
                  "INNER JOIN UsersProducts up " +
                          "ON u.UserId = up.UserId " +
                  "INNER JOIN Products p " +
                          "ON up.ProductId = p.Id " +
                      $"WHERE u.Address = convert(binary(20), '{address}', 1) " +
                        $"AND p.ApiEndpoint = '{productType}';";

            await SqlConnection.ExecuteAsync(sql, new { Quantity = totalQuantity });
        }

        public async Task AddUserProduct(string address, string productType, int quantity)
        {
            var sql = "SELECT u.UserId, p.Id " +
                         "FROM Users u, Products p " +
                       $"WHERE u.Address = convert(binary(20), '{address}', 1) " +
                         $"AND p.ApiEndpoint = '{productType}';";

            (int UserId, int ProductId) response = await SqlConnection.QuerySingleAsync<(int, int)>(sql);

            var sql1 = "INSERT INTO UsersProducts (UserId, ProductId, Quantity) " +
                           $"VALUES ({response.UserId}, {response.ProductId}, {quantity})";

            await SqlConnection.ExecuteAsync(sql1);
        }

        public async Task DeleteUserProduct(string address, string productType)
        {
            var sql = "DELETE FROM UsersProducts " +
                                  "FROM Users u " +
                            "INNER JOIN UsersProducts up " +
                                    "ON u.UserId = up.UserId " +
                            "INNER JOIN Products p " +
                                    "ON up.ProductId = p.Id " +
                                $"WHERE u.Address = convert(binary(20), '{address}', 1) " +
                                  $"AND p.ApiEndpoint = '{productType}';";

            await SqlConnection.ExecuteAsync(sql);
        }
    }
}
