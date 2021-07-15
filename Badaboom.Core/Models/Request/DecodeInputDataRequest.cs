using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Core.Models.Request
{
    public class DecodeInputDataRequest
    {

        public string FunctionAbi { get; set; }

        public string ContractAddress { get; set; }

        public string MethodName { get; set; }

        public string[] FieldNames { get; set; }
        
        public string[] FieldValues { get; set; }
    }
}
