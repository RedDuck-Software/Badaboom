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
        /// Second - startBlock number (optional parameter. Default value - 0)
        /// Third - endBlock number (optional parameter. Default value - CurrentLastBlock)
        /// Default (and only) rpc provider - https://getblock.io service
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("You need to provide at least 2 arguments: network type and LoggerFilePath");

            var _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName)
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .Build();


            var conn = new ConnectionStringsHelperService(_config);

            var getBlockIOPrivateKeys = _config["GetBlockIOPrivateKeys"].Split(",").Select(s => s.Trim()).ToList();

            if (getBlockIOPrivateKeys.Count() == 0) throw new ArgumentException("You must provide at least one private key to use GetBlock rpc provider");

            var rpcProvider = new GetBlockIOProvider(getBlockIOPrivateKeys, args[0]);

            var web3 = new Web3Geth(rpcProvider.GetNextRpcUrl());

            var tracer = new GethWeb3Tracer(web3);

            if (!File.Exists(args[1]))
                File.Create(args[1]);


            var Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.Console()
                                .WriteTo.File(args[1])
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

            var indexer = new Indexer(
                tracer,
                logger,
                    args[0] == "bsc" ?
                    conn.BscDbName :
                    conn.EthDbName,
                rpcProvider,
                500
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

