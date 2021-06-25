using System.Collections.Generic;

namespace Database.Models
{
    public class Block
    {
        public int BlockNumber { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
