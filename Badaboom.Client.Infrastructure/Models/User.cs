using System.Collections.Generic;

namespace Badaboom.Client.Infrastructure.Models
{
    public class User
    {
        public long UserId { get; set; }

        public string AuthToken { get; set; }
        
        public string RefreshToken { get; set; }
        
        public string Address { get; set; }

        public IDictionary<string, int> AvailableProduct { get; set; } = new Dictionary<string, int>();
    }
}
