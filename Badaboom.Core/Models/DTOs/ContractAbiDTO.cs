using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Core.Models.DTOs
{
    public class Input
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("indexed")]
        public bool? Indexed { get; set; }
    }

    public class Output
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ContactAbiDTO
    {
        [JsonProperty("constant")]
        public bool Constant { get; set; }

        [JsonProperty("inputs")]
        public List<Input> Inputs { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("outputs")]
        public List<Output> Outputs { get; set; }

        [JsonProperty("payable")]
        public bool Payable { get; set; }

        [JsonProperty("stateMutability")]
        public string StateMutability { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("anonymous")]
        public bool? Anonymous { get; set; }
    }
}
