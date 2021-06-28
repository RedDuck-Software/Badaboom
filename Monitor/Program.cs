using Database;
using IndexerCore;
using IndexingCore.RpcProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nethereum.Geth;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web3Tracer.Tracers.Geth;
using Serilog;

namespace Monitor
{
    class Program
    {
        private const int DEFAULT_SLEEP_SECONDS = 2;

        static async Task Main(string[] args)
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

            var secondsToSleep = args.Length > 1 ?  Convert.ToInt64(args[1]) : DEFAULT_SLEEP_SECONDS;

            await indexer.StartMonitorNewBlocks(secondsToSleep);
        }
    }
}
