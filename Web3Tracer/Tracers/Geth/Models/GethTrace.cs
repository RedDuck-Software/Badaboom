using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Web3Tracer.Tracers.Geth.Models
{
    internal class GethTrace : GethCall
    {
        [JsonProperty("calls")]
        public IEnumerable<GethCall> Calls { get; set; } = new GethCall[0];
    }
}
