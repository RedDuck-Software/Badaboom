﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Badaboom.Core.Models.DTOs
{
    public class Input
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool? Indexed { get; set; }
    }

    public class Output
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }

    public class Method
    {
        public bool Constant { get; set; }

        public List<Input> Inputs { get; set; }

        public string Name { get; set; }

        public List<Output> Outputs { get; set; }

        public bool Payable { get; set; }

        public string StateMutability { get; set; }

        public string Type { get; set; }

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
