using System.Collections.Generic;

namespace Database.Models
{
    public class Block
    {
        public long BlockNumber { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
