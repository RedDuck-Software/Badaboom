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
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

            string sectionName;
#if (DEBUG)
    sectionName = "ArgsDebug";
#else
    sectionName = "ArgsRelease";
#endif

            var _args = config?.GetSection(sectionName);
            if (_args == null)
                throw new ArgumentException(sectionName + " section is expected in the config");

            var LogFile = _args["LogFile"];
            var LogCriticalFile = _args["LogCriticalFile"];
            var sBlockQueueSize = _args["BlockQueueSize"];
            var sStartBlock = _args["StartBlock"];
            var sEndBlock = _args["EndBlock"];
            var RPCProvider = _args["RPCProvider"];
            var LoadingInnerCalls = _args["LoadingInnerCalls"];
            var StartIndexing = _args["StartIndexing"];

            if (LogFile == null)
                throw new ArgumentException("LogFile parameter is expected");

            if (LogCriticalFile == null)
                throw new ArgumentException("LogCriticalFile parameter is expected");

            if (sBlockQueueSize == null)
                throw new ArgumentException("BlockQueueSize parameter is expected");

            if (RPCProvider == null)
                throw new ArgumentException("RPCProvider parameter is expected");

            if (LoadingInnerCalls == null)
                throw new ArgumentException("LoadingInnerCalls parameter is expected");

            var conn = new ConnectionStringsHelperService(config);

            var rpcUrl = RPCProvider;

            var web3 = new Web3Geth(rpcUrl);

            var tracer = new GethWeb3Tracer(web3);

            if (!File.Exists(LogFile))
                File.Create(LogFile);

            if (!File.Exists(LogCriticalFile))
                File.Create(LogCriticalFile);

            var loadingInnerCalls = Convert.ToBoolean(LoadingInnerCalls);

            List<string> addressesToIndexList = null;

            var Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.Console()
                                .WriteTo.File(LogFile)
                                .WriteTo.File(LogCriticalFile, Serilog.Events.LogEventLevel.Fatal)
                                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddLogging(config =>
                {
                    config.AddSerilog(Logger, true);
                })
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                  .CreateLogger<Program>();

            var blockQueueSize = Convert.ToInt32(sBlockQueueSize);
            if (blockQueueSize < 1) throw new ArgumentException("BlockQueueSize must be greater than zero");

            var indexer = new Indexer(
                tracer,
                logger,
                conn.CrimeChain,
                blockQueueSize,
                loadingInnerCalls
            );

            var startBlock = sStartBlock != null ? long.Parse(sStartBlock) : 0;
            var endBlock = sEndBlock != null ? long.Parse(sEndBlock) : (long)await indexer.GetLatestBlockNumber();
            if (endBlock < 1)
                endBlock = (long)await indexer.GetLatestBlockNumber();

            var startIndexing = StartIndexing != null ? Boolean.Parse(StartIndexing) : false;
            if (startIndexing)
            {
                await indexer.IndexInRangeParallel(startBlock, endBlock, 50);
                ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");
            }

            ConsoleColor.Magenta.WriteLine("\nStarting indexing to latest block"); 
            await indexer.StartMonitorNewBlocks(1000);
        }
    }
}
