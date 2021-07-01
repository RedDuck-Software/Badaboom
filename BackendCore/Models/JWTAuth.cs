﻿using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend.Models
{
    public class JWTAuth
    {
        public string Issuer { get; set; }  
        public string Audience { get; set; } 
        public string Key { get; set; }
        public int Lifetime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
            => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.Key));
    }
}