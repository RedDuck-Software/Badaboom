using Nethereum.Geth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IndexingCore.RpcProviders
{
    public class GetBlockIOProvider
    {
        public List<string> ApiKeys { get; }

        public bool IsAllTokensUsed { get; private set; }


        public GetBlockIOProvider(IEnumerable<string> apiKeys, string network)
        {
            ApiKeys = new List<string>(apiKeys);
            _network = network;
        }

        public string GetNextRpcUrl() => String.Format(_urlBaseTemplate, _network, GetNextApiKey());


        public void Reset() => (IsAllTokensUsed, _currentKeyPosition) = (false, 0);

        private string GetNextApiKey()
        {
            if (_currentKeyPosition >= ApiKeys.Count)
            {
                IsAllTokensUsed = true;
                _currentKeyPosition = 0; // looping through all keys again
            }

            return ApiKeys[_currentKeyPosition++];
        }

        private static readonly string _urlBaseTemplate = "https://{0}.getblock.io/?api_key={1}";

        private readonly string _network;

        private int _currentKeyPosition = 0;
    }
}
