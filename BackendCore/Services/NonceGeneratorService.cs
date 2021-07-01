using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendCore.Services
{
    public class NonceGeneratorService
    {
        public int NonceLenght { get; }



        public NonceGeneratorService(int nonceLenght)
        {
            NonceLenght = nonceLenght;
        }

        public string GenerateNonce()
        {
            return new string(Enumerable.Repeat(chars, NonceLenght)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private readonly Random _random = new();
        
        private readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghigklmnopqrstuvwxyz-!#$%^&";
    }
}
