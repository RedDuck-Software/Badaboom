using Dapper;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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
                      $"WHERE u.Address = convert(binary(20), @Address, 1) " +
                        $"AND p.ApiEndpoint = @ProductType;";

            return await SqlConnection.QuerySingleOrDefaultAsync<int?>(sql, new { Address = address, ProductType = productType });
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
                      $"WHERE u.Address = convert(binary(20), @Address, 1) " +
                        $"AND p.ApiEndpoint = @ProductType;";

            await SqlConnection.ExecuteAsync(sql, new { Quantity = totalQuantity, Address = address, ProductType = productType });
        }

        public async Task AddUserProduct(string address, string productType, int quantity)
        {
            var sql = "SELECT u.UserId, p.Id " +
                        "FROM Users u, Products p " +
                      $"WHERE u.Address = convert(binary(20), @Address, 1) " +
                        $"AND p.ApiEndpoint = @ProductType;";

            (int UserId, int ProductId) response = await SqlConnection.QuerySingleAsync<(int, int)>(sql, new { Address = address, ProductType = productType });

            var sql1 = "INSERT INTO UsersProducts (UserId, ProductId, Quantity) " +
                           $"VALUES (@V1, @V2, @V3)";

            await SqlConnection.ExecuteAsync(sql1, new { V1 = response.UserId, V2 = response.ProductId, V3 = quantity });
        }

        public async Task DeleteUserProduct(string address, string productType)
        {
            var sql = "DELETE FROM UsersProducts " +
                             "FROM Users u " +
                       "INNER JOIN UsersProducts up " +
                               "ON u.UserId = up.UserId " +
                       "INNER JOIN Products p " +
                               "ON up.ProductId = p.Id " +
                           $"WHERE u.Address = convert(binary(20), @Address, 1) " +
                             $"AND p.ApiEndpoint = @ProductType;";

            await SqlConnection.ExecuteAsync(sql, new { Address = address, ProductType = productType });
        }

        public async Task<(long, Dictionary<int, byte>)> GetProductPrice(string productType)
        {
            var sql = "SELECT p.Price, pd.AmountFrom, pd.Percents " +
                        "FROM Products p, ProductsDiscount pd " +
                      $"WHERE p.ApiEndpoint = @ProductType;";

            var response = await SqlConnection.QueryAsync<(long price, int amountFrom, byte percents)>(sql, new { ProductType = productType });

            long price = 0;

            Dictionary<int, byte> AmountFromPercents = new Dictionary<int, byte>();

            foreach (var row in response)
            {
                price = row.price;

                if (!AmountFromPercents.ContainsKey(row.amountFrom))
                {
                    AmountFromPercents.Add(row.amountFrom, row.percents);
                }
            }

            return (price, AmountFromPercents);
        }
    }
}
