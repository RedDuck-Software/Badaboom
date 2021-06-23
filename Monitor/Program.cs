using System;
using System.IO;
using System.Threading.Tasks;
using Database;
using IndexerCore;
using Microsoft.Extensions.Configuration;
using Nethereum.Geth;
using Web3Tracer.Tracers.Geth;

namespace Monitor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var web3 = new Web3Geth(args[1]);

            var tracer = new GethWeb3Tracer(web3);

            var _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName)
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .Build();

            var conn = new ConnectionStringsHelperService(_config);

            var indexer = new Indexer(
                tracer,null,
                    args[0] == "bsc" ?
                    conn.BscDbName :
                    conn.EthDbName
            );

            await indexer.StartMonitorNewBlocks(2);
        }
    }
}
