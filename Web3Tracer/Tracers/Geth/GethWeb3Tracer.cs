using Nethereum.Parity;
using Nethereum.Web3;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web3Tracer.Models;
using Web3Tracer.Tracers.Parity.Models;
using Web3Tracer.Extensions;
using Nethereum.Geth;
using Nethereum.Geth.RPC.Debug.DTOs;
using Newtonsoft.Json.Linq;
using System.Net;
using Nethereum.JsonRpc.Client;
using Web3Tracer.Tracers.Geth.Models;

namespace Web3Tracer.Tracers.Geth
{
    public class GethWeb3Tracer : IWeb3Tracer
    {
        public Web3 Web3 { get => _web3Geth; }


        public GethWeb3Tracer(Web3Geth web3Parity)
        {
            _web3Geth = web3Parity;
        }


        public async Task<IEnumerable<TraceResult>> GetTracesForTransaction(string txHash)
        {
            var rawTrace = await GetDebugTraceFromRpcCall(txHash);

            var trace = rawTrace.ToObject<GethTrace>();

            List<TraceResult> calls = new List<TraceResult>();

            calls.Add(trace.ToTraceResult());

            foreach (var call in trace.Calls)
                calls.Add(call.ToTraceResult());


            return calls;
        }


        private async Task<JObject> GetDebugTraceFromRpcCall(string txHash)
        {
            var cl = new RpcClient(new System.Uri("http://localhost:8545"));

            return await cl.SendRequestAsync<JObject>(new RpcRequest(null, "debug_traceTransaction",
                new object[]{
                    txHash,
                    new {
                        tracer = "callTrace"
                    }
                }));
        }

        private Web3Geth _web3Geth;
    }
}
