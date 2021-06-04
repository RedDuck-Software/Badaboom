using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web3Tracer.Tracers.Parity.Models
{
    internal class ParityTraceAction
    {
        [JsonProperty("callType")]
        public string CallType { get; set; }

        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("gas")]
        public string GasUsed { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}

