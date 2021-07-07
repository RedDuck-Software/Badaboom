using Database.Models;

namespace BackendCore.Models
{
    public class AuthenticateResponse
    {
        public long UserId { get; set; }

        public string AuthToken { get; set; }

        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            UserId = user.UserId;
            AuthToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
