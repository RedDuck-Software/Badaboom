using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientCore.Models
{
    public class Transaction
    {
        public string TxnHash { get; set; }
        public string Method { get; set; }
        public uint Block { get; set; }
        public uint Age { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Value { get; set; }
        public decimal TxnFee { get; set; }
    }
}