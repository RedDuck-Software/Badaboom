using Newtonsoft.Json;

namespace Web3Tracer.Tracers.Parity.Models
{
    class ParityTraceResult
    {
        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }
    }
}
