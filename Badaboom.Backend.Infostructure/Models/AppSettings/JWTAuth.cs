using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend.Models
{
    public class JWTAuth
    {
        public string Issuer { get; set; }  
        public string Audience { get; set; } 
        public string Secret { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int RefreshTokenLifetime { get; set; }


        public SymmetricSecurityKey GetSymmetricSecurityKey()
            => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.Secret));
    }
}
