using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackendCore.Models
{
    public class AuthenticateResponse
    {
        public long Id { get; set; }
        public string UserAddress { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            Id = user.UserId;
            UserAddress = user.Address;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
