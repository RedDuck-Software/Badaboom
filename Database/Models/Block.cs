using System.Collections.Generic;

namespace Database.Models
{
    public class Block
    {
        public long BlockNumber { get; set; }

        public string IndexingStatus { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
