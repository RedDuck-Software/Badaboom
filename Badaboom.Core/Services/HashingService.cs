using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Core.Services
{
    public class HashingService
    {
        public static string SHA3(string val) => (new Nethereum.Util.Sha3Keccack()).CalculateHash(val);

        public static string EncodeMethodSignature(string sign, bool prefixed = true) => (prefixed ? "0x" : "") + SHA3(sign).Substring(0, 8);
    }
}
