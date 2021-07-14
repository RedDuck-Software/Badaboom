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
            if (args.Length < 4) throw new ArgumentException("You need to provide at least 4 arguments: network type, LoggerFilePath, LoggerCriticalFilePath and BlockQueueSize");

            var _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName)
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .Build();


            var conn = new ConnectionStringsHelperService(_config);

            var inuraPrivateKeys = _config["InfuraApiKeys"].Split(",").Select(s => s.Trim()).ToList();

            if (inuraPrivateKeys.Count() == 0) throw new ArgumentException("You must provide at least one private key to use Infura rpc provider");

            var rpcProvider = new InfuraProvider(inuraPrivateKeys, args[0]);

            var web3 = new Web3Geth(rpcProvider.GetNextRpcUrl());

            var tracer = new GethWeb3Tracer(web3);

            if (!File.Exists(args[1]))
                File.Create(args[1]);


            if (!File.Exists(args[2]))
                File.Create(args[2]);

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

            serviceProvider
                   .GetService<ILoggerFactory>();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                  .CreateLogger<Program>();


            var blockQueueSize = Convert.ToInt32(args[3]);

            if (blockQueueSize < 1) throw new ArgumentException("BlockQueueSize must be greater than zero");

            var t = rpcProvider.GetNextRpcUrl();


            var indexer = new Indexer(
                tracer,
                logger,
                    args[0] == "bsc" ?
                    conn.BscDbName :
                    conn.EthDbName,
                rpcProvider,
                blockQueueSize
            );

            var startBlock = args.Length > 4 ? ulong.Parse(args[4]) : 0;

            var endBlock = args.Length > 5 ? ulong.Parse(args[5]) : await indexer.GetLatestBlockNumber();

            await indexer.IndexInRangeParallel(startBlock, endBlock, 20);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");

            ConsoleColor.DarkMagenta.WriteLine("\n\nStarting getting new blocks...\n\n");
        }
    }
}

