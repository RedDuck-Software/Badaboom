using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings;
using System.Text.Json;

namespace Badaboom.Core.Models.Response
{
    public class UserResponce
    {
        public long UserId { get; set; }

        public string Address { get; set; }

        public string Nonce { get; set; }
    }
}
