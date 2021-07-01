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


    }
}
