using System.Threading.Tasks;
using System;
using Web3Tracer.Tracers.Geth;
using Nethereum.Geth;
using Database;
using IndexerCore;
using IndexerCore.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BadaboomIndexer
{
    class Program
    {

        /// <summary>
        /// First arg - string, possible values: bsc | eth . Responsible for chain selection
        /// Second args element must be web3 provider url
        /// Third - startBlock number
        /// Fourth - endBlock number
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var web3 = new Web3Geth(args[1]);

            var tracer = new GethWeb3Tracer(web3);

            var _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName)
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .Build();

            var conn = new ConnectionStringsHelperService(_config);

            var indexer = new Indexer(
                tracer,
                    args[0] == "bsc" ?
                    conn.BscDbName :
                    conn.EthDbName
            );

            var startBlock = args.Length > 2 ? ulong.Parse(args[2]) : 0;

            var endBlock = args.Length > 3 ? ulong.Parse(args[3]) : await indexer.GetLatestBlockNumber();

            await indexer.IndexInRangeParallel(startBlock, endBlock, 20);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");

            ConsoleColor.DarkMagenta.WriteLine("\n\nStarting getting new blocks...\n\n");


            // Run new block monitoring
            await indexer.StartMonitorNewBlocks(1);
        }
    }
}
