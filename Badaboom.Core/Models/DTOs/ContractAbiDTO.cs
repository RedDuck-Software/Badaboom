using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Badaboom.Core.Models.DTOs
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
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

    public class Method
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

    public class AbiInputComparer: IEqualityComparer<Input>
    {
        public bool Equals(Input x, Input y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] Input obj)
        {
            return obj.GetHashCode();
        }
    }
}
