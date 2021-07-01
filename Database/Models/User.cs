﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Database.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Address { get; set; }

        public string Nonce { get; set; }
    }
}
