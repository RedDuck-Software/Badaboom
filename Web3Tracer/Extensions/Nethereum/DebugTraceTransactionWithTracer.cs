using Nethereum.Geth.RPC;
using Nethereum.Geth.RPC.Debug;
using Nethereum.JsonRpc.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Web3Tracer.Tracers.Geth.Models;

namespace Web3Tracer.Extensions.Nethereum
{
    public static class DebugTraceTransactionWithTracer
    {
        public static Task<JObject> SendRequestAsync(this DebugTraceTransaction debugTracer,string txHash, TraceTransactionOptionWithTracer traceOption,object id = null)
        {
            if (debugTracer.Client == null) throw new NullReferenceException("RpcRequestHandler Client is null");

            var request = debugTracer.BuildRequest(id, txHash, traceOption);

            return debugTracer.Client.SendRequestAsync<JObject>(request);
        }
    }

}
