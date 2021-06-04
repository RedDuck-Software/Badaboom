using Nethereum.Parity;
using Nethereum.Web3;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web3Tracer.Models;
using Web3Tracer.Tracers.Parity.Models;
using Web3Tracer.Extensions;

namespace Web3Tracer.Tracers.Parity
{
    public class ParityWeb3Tracer : IWeb3Tracer
    {
        public Web3 Web3 { get => _web3Parity; }


        public ParityWeb3Tracer(Web3Parity web3Parity)
        {
            _web3Parity = web3Parity;
        }
        
        
        public async Task<IEnumerable<TraceResult>> GetTracesForTransaction(string txHash)
        {
            var rawTrace = await _web3Parity.Trace.TraceTransaction.SendRequestAsync(txHash);
            var trace = rawTrace.ToObject<IEnumerable<ParityTrace>>();

            return trace.Select(x => x.ToTraceResult());
        }

        private Web3Parity _web3Parity;
    }
}
