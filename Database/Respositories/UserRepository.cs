using Dapper;
using Database.Models;
using Database.Models.BinaryModels;
using Database.Models.BinaryModels.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            var sql = "insert into RefreshTokens(UserId,Token,Expires,Created,Revoked,ReplacedByToken,CreatedByIp,RevokedByIp) " +
                "values(@UserId,@Token,@Expires,@Created,@Revoked,@ReplacedByToken,@CreatedByIp,@RevokedByIp);";

            await SqlConnection.QuerySingleAsync<User>(sql, newToken);
        }


        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }


        public async Task<RefreshToken> GetRefreshToken(string address)
        {
            throw new NotImplementedException();
            /*var sql = "select from Users where Address=@Address;";
            return await SqlConnection.QuerySingleAsync<User>(sql, new { Address = address });*/
        }
    }
}
