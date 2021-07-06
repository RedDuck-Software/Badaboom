using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Models
{
    public class RefreshToken
    {
        public long TokenId { get; set; }

        public long UserId { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }

        public string CreatedByIp { get; set; }
        public string RevokedByIp { get; set; }

        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public User User { get; set; }
    }
}
