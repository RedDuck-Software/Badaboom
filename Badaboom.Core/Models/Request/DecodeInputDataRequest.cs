using Badaboom.Core.Models.DTOs;
using System.Collections.Generic;

namespace Badaboom.Core.Models.Request
{
    public class DecodeInputDataRequest
    {
        public IEnumerable<Method> FunctionAbis { get; set; }

        /// <summary>
        /// Key - argument name, Value - argument value
        /// </summary>
        public Dictionary<string, string> ArgumentsNamesValues { get; set; }
    }
}
