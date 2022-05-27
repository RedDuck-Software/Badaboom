using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Database
{
    public class ConnectionStringsHelperService
    {
        public ConnectionStringsHelperService(IConfigurationRoot config) => (_config) = (config);

        public string CrimeChain => _config.GetConnectionString("CrimeChain");
     
        private IConfigurationRoot _config;
    }
}
