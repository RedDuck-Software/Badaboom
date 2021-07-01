using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Models.BinaryModels
{
    internal class UserBinary
    {
        public long Id { get; set; }

        public byte[] Address { get; set; }

        public string Nonce { get; set; }
    }
}
