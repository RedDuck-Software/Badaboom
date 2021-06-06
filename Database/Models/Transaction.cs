using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public long BlockId { get; set; }

        public string Hash { get; set; }

        public DateTime Time { get; set; }

        public IEnumerable<Call> Calls { get; set; }

        public RawTransaction RawTransaction { get; set; }
    }

    public class RawTransaction
    {
        public string From { get; set; }

        public string To { get; set; }

        public string MethodId { get; set; }

        public string Value { get; set; }

        public string Input { get; set; }

        public ulong Gas { get; set; }

        public ulong GasPrice { get; set; }
    }
}
