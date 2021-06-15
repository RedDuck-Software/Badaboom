using Nethereum.Geth.RPC.Debug.DTOs;
using Newtonsoft.Json;

namespace Web3Tracer.Tracers.Geth.Models
{
    public class TraceTransactionOptionWithTracer : TraceTransactionOptions
    {
        [JsonProperty("tracer")]
        public string Tracer { get; set; }
    }

    public enum TransactionTracer { callTracer }
}
