using System.Collections.Generic;

namespace Badaboom.Core.Models.Request
{
    public class DecodeInputDataRequest
    {

        public string FunctionAbi { get; set; }

        public string ContractAddress { get; set; }

        public string MethodName { get; set; }

        /// <summary>
        /// Key - argument name, Value - argument value
        /// </summary>
        public Dictionary<string, string> ArgumentsNamesValues { get; set; }
    }
}
