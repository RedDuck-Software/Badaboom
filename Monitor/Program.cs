using Database;
using IndexerCore;
using IndexingCore.RpcProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nethereum.Geth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndexerCore.Extensions;
using Web3Tracer.Tracers.Geth;
using Serilog;

namespace Monitor
{
    class Program
    {
        private const int DEFAULT_SLEEP_SECONDS = 2;

        static async Task Main(string[] args)
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
                addressesToIndexList = (await File.ReadAllLinesAsync(addressesToIndexFilePath)).Select(x=>x.Trim()).ToList();
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

            await indexer.StartMonitorNewBlocks(DEFAULT_SLEEP_SECONDS);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");
        }
    }
}
