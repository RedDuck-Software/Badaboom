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

            var indexer = new Indexer(
                tracer,
                null,
                    args[0] == "bsc" ?
                    conn.BscDbName :
                    conn.EthDbName,
                rpcProvider,
                500
            );

            var startBlock = args.Length > 1 ? ulong.Parse(args[1]) : 0;

            var endBlock = args.Length > 2 ? ulong.Parse(args[2]) : await indexer.GetLatestBlockNumber();

            await indexer.IndexInRangeParallel(startBlock, endBlock, 20);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");

            ConsoleColor.DarkMagenta.WriteLine("\n\nStarting getting new blocks...\n\n");

            // Run new block monitoring
            await indexer.StartMonitorNewBlocks(1);
        }
    }
}
