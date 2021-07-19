using System.Threading.Tasks;
using System;
using Web3Tracer.Tracers.Geth;
using Nethereum.Geth;
using Database;
using IndexerCore;
using IndexerCore.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using IndexingCore.RpcProviders;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BadaboomIndexer
{
    class Program
    {
        /// <summary>
        /// First arg - string, possible values: bsc | eth . Responsible for chain selection
        /// Second arg - Log file
        /// Third arg - LogCritical file
        /// Fourth arg - block queue size
        /// Fifth - startBlock number (optional parameter. Default value - 0)
        /// Six - endBlock number (optional parameter. Default value - CurrentLastBlock)
        /// Default (and only) rpc provider - https://getblock.io service
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            if (args.Length < 6) throw new ArgumentException("You need to provide at least 6 arguments: network type, LoggerFilePath, LoggerCriticalFilePath, RPC url, bool:indexInnerCalls and BlockQueueSize");

            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .Build();


            var conn = new ConnectionStringsHelperService(config);

            var rpcUrl = args[3];

            var web3 = new Web3Geth(rpcUrl);

            var tracer = new GethWeb3Tracer(web3);

            if (!File.Exists(args[1]))
                File.Create(args[1]);

            if (!File.Exists(args[2]))
                File.Create(args[2]);


            var loadingInnerCalls = Convert.ToBoolean(args[4]);

            var addressesToIndexFilePath = Path.Combine(Environment.CurrentDirectory, "AddressesToIndex.txt");

            List<string> addressesToIndexList = null;

            if(File.Exists(addressesToIndexFilePath))
            {
                addressesToIndexList = File.ReadAllLines(addressesToIndexFilePath).Select(x=>x.Trim()).ToList();
                ConsoleColor.Magenta.WriteLine("\nIndexing only selected addresses");
            }


            var Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.Console()
                                .WriteTo.File(args[1])
                                .WriteTo.File(args[2], Serilog.Events.LogEventLevel.Fatal)
                                .CreateLogger();


            var serviceProvider = new ServiceCollection()
                .AddLogging(config =>
                {
                    config.AddSerilog(Logger, true);
                })
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                  .CreateLogger<Program>();


            var blockQueueSize = Convert.ToInt32(args[5]);

            if (blockQueueSize < 1) throw new ArgumentException("BlockQueueSize must be greater than zero");

            var indexer = new Indexer(
                tracer,
                logger,
                    args[0] == "bsc" ?
                    conn.BscDbName :
                    conn.EthDbName,
                blockQueueSize,
                loadingInnerCalls,
                addressesToIndexList
            );

            var startBlock = args.Length > 6 ? long.Parse(args[6]) : 0;

            var endBlock = args.Length > 7 ? long.Parse(args[7]) : (long)await indexer.GetLatestBlockNumber();

            await indexer.IndexInRangeParallel(startBlock, endBlock, 50);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");
        }
    }
}

