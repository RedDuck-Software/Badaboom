using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom_indexer.Models
{


    public class ParityTrace
    {
        [JsonProperty("action")]
        public ParityTraceAction Action { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        
    }

    public class ParityTraceAction
    { 
        [JsonProperty("callType")]
        public string CallType { get; set; }

        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }
}
