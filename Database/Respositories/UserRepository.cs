using Dapper;
using Database.Models;
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
            var sql = "insert into Users(Address,Nonce) values(@Address,@Nonce);";
            await SqlConnection.ExecuteAsync(sql, user);
        }

        public async Task<User> GetUserByAddress(string address)
        {
            var sql = "select from Users where Address=@Address;";
            return await SqlConnection.QuerySingleAsync<User>(sql, new { Address = address});
        }
            
        public async Task AddNewRefreshToken(User user, RefreshToken newToken)
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
