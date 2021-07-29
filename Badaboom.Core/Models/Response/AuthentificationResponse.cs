using Database.Models;
using System.Collections.Generic;

namespace BackendCore.Models
{
    public class AuthenticateResponse
    {
        public long UserId { get; set; }

        public string AuthToken { get; set; }

        public string RefreshToken { get; set; }

        public IDictionary<string, int> AvailableProduct { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            UserId = user.UserId;
            AuthToken = jwtToken;
            RefreshToken = refreshToken;
            AvailableProduct = user.AvailableProduct;
        }
    }
}
