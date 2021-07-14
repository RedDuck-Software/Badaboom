using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Client.Infrastructure.Models
{
    public class TransactionInputData
    {
        public string MethodId { get; set; }

        public List<string> Parametrs { get; set; } = new();
    }
}
