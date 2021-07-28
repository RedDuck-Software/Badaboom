using Badaboom.Core.Models.Enums;
using System.Numerics;

namespace Badaboom.Core.Models.Response
{
    public class InternalTxnResponse
    {
        public CallTypes Type { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public BigInteger Value { get; set; }

        public BigInteger GasLimit { get; set; }
    }
}
