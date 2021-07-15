using System.Collections.Generic;

namespace Badaboom.Core.Models.Request
{
    public class DecodeInputDataRequest
    {

        public string FunctionAbi { get; set; }

        public string ContractAddress { get; set; }

        public string MethodName { get; set; }

        public List<string> FieldNames { get; set; }

        public List<string>  FieldValues { get; set; }
    }
}
