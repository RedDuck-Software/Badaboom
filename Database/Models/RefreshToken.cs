using System;

namespace Database.Models
{
    public class RefreshToken
    {
        public long TokenId { get; set; }

        public long UserId { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }

        public string CreatedByIp { get; set; }

        public bool IsActive => !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public User User { get; set; }
    }
}
