using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Database.Models
{
    public class User
    {
        public long UserId { get; set; }

        public string Address { get; set; }

        public string Nonce { get; set; }

        public int ArgumentFunctionRequests { get; set; }

        public IDictionary<string, int> AvailableProduct { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
