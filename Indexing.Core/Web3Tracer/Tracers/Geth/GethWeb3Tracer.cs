using Nethereum.Web3;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web3Tracer.Models;
using Web3Tracer.Extensions;
using Web3Tracer.Extensions.Nethereum;
using Nethereum.Geth;
using Web3Tracer.Tracers.Geth.Models;
using Nethereum.Geth.RPC.Debug;

namespace Web3Tracer.Tracers.Geth
{
    public class GethWeb3Tracer : IWeb3Tracer
    {
        public Web3 Web3 { get => _web3Geth; }


        public GethWeb3Tracer(Web3Geth web3geth)
        {
            _web3Geth = web3geth;
        }


        public async Task<IEnumerable<TraceResult>> GetTracesForTransaction(string txHash)
        {
            var options = new TraceTransactionOptionWithTracer { Tracer = "callTracer" };

            var rawTrace = await ((DebugTraceTransaction)_web3Geth.Debug.TraceTransaction).SendRequestAsync(txHash, traceOption: options);

            if (rawTrace is null) return null;

            var trace = rawTrace.ToObject<GethTrace>();

            List<TraceResult> calls = new List<TraceResult>();

            calls.Add(trace.ToTraceResult());

            if (trace.Calls != null)
                foreach (var call in trace.Calls)
                    calls.Add(call.ToTraceResult());

            return calls;
        }

        public void ChangeWeb3Provider(string newRpcUrl)
        {
            _web3Geth = new Web3Geth(newRpcUrl);

        }

        private Web3Geth _web3Geth;
    }
}
