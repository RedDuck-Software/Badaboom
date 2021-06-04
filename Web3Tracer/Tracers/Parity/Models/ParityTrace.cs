using Newtonsoft.Json;

namespace Web3Tracer.Tracers.Parity.Models
{
    internal class ParityTrace
    {
        [JsonProperty("action")]
        public ParityTraceAction Action { get; set; }

        [JsonProperty("result")]
        public ParityTraceResult Result { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
