using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace AzureIndexer
{
    public static class StartIndexingProcess
    {
        [FunctionName("StartIndexingProcess")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            /*var conn = new ConnectionStringsHelperService(_config);

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
            await indexer.StartMonitorNewBlocks(1);*/

            return new OkResult();
        }
    }
}
