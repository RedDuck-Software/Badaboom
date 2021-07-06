using Dapper;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Database.Respositories
{
    public class UserRepository : RepositoryBase
    {
        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task CreateUser(User user)
        {
            var sql = $"insert into Users(Address,Nonce) values(convert(binary(20),'{user.Address}',1),@Nonce);";
            await SqlConnection.ExecuteAsync(sql, user);
        }

        public async Task<User> GetUserByAddress(string address)
        {
            var sql = "select " +
                            "UserId, " +
                            "convert(varchar(42), Address, 1) as Address," +
                            "Nonce " +
                      "from Users u " +
                      "where " +
                            $"Address=convert(binary(20),'{address}',1);";

            return await SqlConnection.QuerySingleOrDefaultAsync<User>(sql);
        }

        public async Task AddNewRefreshToken(RefreshToken newToken)
        {
            var sql = "insert into RefreshTokens(UserId,Token,Expires,Created,CreatedByIp) " +
                "values(@UserId,@Token,@Expires,@Created,@CreatedByIp);";

            await SqlConnection.ExecuteAsync(sql, newToken);
        }


        public async Task<RefreshToken> GetRefreshTokenWithUser(string refreshToken)
        {
            var sql = "select " +
                            "r.TokenId, r.UserId,r.Token,r.Expires,r.Created,r.CreatedByIp, " +
                            "u.UserId, " +
                            "convert(varchar(42), u.Address, 1) as Address, " +
                            "u.Nonce " +
                      "from RefreshTokens r " +
                      "inner join Users u on r.UserId=u.UserId";

            return (await SqlConnection.QueryAsync<RefreshToken, User, RefreshToken>(sql, (refreshToken, user) =>
            {
                refreshToken.User = user;
                return refreshToken;
            }, splitOn: "UserId"))?.FirstOrDefault();
        }

        public async Task RemoveRefreshToken(string token)
        {
            var sql = "delete " +
                        "from RefreshTokens " +
                        "where Token=@Token";

            await SqlConnection.ExecuteAsync(sql, new { Token = token });
        }
    }
}
