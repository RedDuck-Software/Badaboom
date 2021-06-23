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

/*var vaultService = new KeyVaultService();

           string network = req.Query["network"];

           string queueSize = req.Query["queueMaxSize"];

           string startBlockP = req.Query["startBlock"];

           string endBlockP = req.Query["endBlock"];


           if (String.IsNullOrEmpty(network)) return new BadRequestObjectResult(new { error = "You must provide {network} parameter" });

           var getBlockIOPrivateKeys =    *//*(await vaultService.GetSecretValue(""))*//* "72e5b074-9416-43a8-bef8-ebb70cc6c8d0, bb9d169f-94b6-4c53-b2bd-bc76eac1bbb8, f26236b1-b5d7-475d-8eec-c99753ee8575, d43fe739-68e1-46bb-801f-20b1a9503e04, 18c4b90f-2f78-4f91-b4ee-5d4aaac07d64, 8b0a993d-f76b-44a9-898a-da65374bd980, 779d8ceb-f58f-49f6-b269-08ac781e46a8, e0851b96-6411-4266-a347-475cdbdf3ddb, af5c57cb-21d0-4ee1-b81c-2b8d6959a9f3, 91c5c459-4c5c-4f03-b585-cf165303da96, 229a2742-031f-4861-a870-3bb4bb65fe7b, 2f0f0388-e1c6-4e38-bff4-1ca1ee447d5f, 959a32bb-dbfa-4e2c-a282-265f743d4795"
               .Split(",").Select(s => s.Trim()).ToList();


           if (getBlockIOPrivateKeys.Count() == 0) return new BadRequestObjectResult(new { error = "You must provide at least one private key to use GetBlock rpc provider" });


           var rpcProvider = new GetBlockIOProvider(getBlockIOPrivateKeys, network);

           var web3 = new Web3Geth(rpcProvider.GetNextRpcUrl());

           var tracer = new GethWeb3Tracer(web3);

           var indexer = new Indexer(
               tracer,
               log,
                   network == "bsc" ?
                   ConfigurationManager.ConnectionStrings["BSC"].ConnectionString :
                   ConfigurationManager.ConnectionStrings["ETH"].ConnectionString,
               rpcProvider,
               String.IsNullOrEmpty(queueSize) ? 500 : Convert.ToInt32(queueSize)
           );

           var startBlock = String.IsNullOrEmpty(startBlockP) ? ulong.Parse(startBlockP) : 0;

           var endBlock = String.IsNullOrEmpty(endBlockP) ? ulong.Parse(endBlockP) : await indexer.GetLatestBlockNumber();


           await indexer.IndexInRangeParallel(startBlock, endBlock, 20);

           log.LogInformation("\nIndexing successfully done!");*/