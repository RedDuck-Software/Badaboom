using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientCore.Models;

namespace ClientCore.Models
{
    public class PaginationTransactionResponse
    {
        public int Count { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
