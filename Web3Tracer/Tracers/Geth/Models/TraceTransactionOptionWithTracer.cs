using Nethereum.Geth.RPC.Debug.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web3Tracer.Tracers.Geth.Models
{
    public class TraceTransactionOptionWithTracer : TraceTransactionOptions
    {
        [JsonProperty("tracer")]
        public string Tracer { get; set; }
    }

    public enum TransactionTracer { callTracer }
}
