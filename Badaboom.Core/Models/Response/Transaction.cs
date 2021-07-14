using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badaboom.Core.Models.Response
{
    public class Transaction
    {
        public string TxnHash { get; set; }
        
        public string Method { get; set; }
        
        public ulong Block { get; set; }
        
        public ulong Age { get; set; }
        
        public string From { get; set; }

        public string Input { get; set; }

        public string To { get; set; }
        
        public decimal Value { get; set; }
 
        public decimal TxnFee { get; set; }
    }
}