using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Database
{
    public class ConnectionStringsHelperService
    {
        public ConnectionStringsHelperService(IConfigurationRoot config) => (_config) = (config);


        public string BscDbName => _config.GetConnectionString("BSC");

        public string EthDbName => _config.GetConnectionString("ETH");

     
        private IConfigurationRoot _config;
    }
}
