using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class Transaction
    {
        public string TransactionHash { get; set; }

        public int BlockId { get; set; }

        public int TimeStamp { get; set; }

        public List<Call> Calls { get; set; }
        
        public RawTransaction RawTransaction { get; set; }
    }

    public class RawTransaction
    {
        public string From { get; set; }

        public string To { get; set; }

        public string MethodId { get; set; }

        public string Value { get; set; }
    }
}
