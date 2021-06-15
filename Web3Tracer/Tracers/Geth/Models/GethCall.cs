using Newtonsoft.Json;

namespace Web3Tracer.Tracers.Geth.Models
{
    internal class GethCall
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("gas")]
        public string Gas { get; set; }

        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
