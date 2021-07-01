using System;
using System.Linq;

namespace BackendCore.Services
{
    public interface INonceGeneratorService
    {
        int NonceLenght { get; }

        string GenerateNonce();
    }

    public class NonceGeneratorService : INonceGeneratorService
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
