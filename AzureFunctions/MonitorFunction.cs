using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using IndexingCore.RpcProviders;
using Nethereum.Geth;
using Web3Tracer.Tracers.Geth;
using IndexerCore;
using System.Configuration;

namespace AzureFunctions
{
    public static class MonitorFunction
    {
        [FunctionName("MonitorAndIndexNewBlocks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string network = req.Query["network"];

            string queueSize = req.Query["queueMaxSize"];

            string startBlockP = req.Query["startBlock"];

            string endBlockP = req.Query["endBlock"];


            if (String.IsNullOrEmpty(network)) return new BadRequestObjectResult(new { error = "You must provide {network} parameter" });

            var getBlockIOPrivateKeys = Environment.GetEnvironmentVariable("BlockioPrivateKeys").Split(",").Select(s => s.Trim()).ToList();

            foreach (var str in getBlockIOPrivateKeys)
                log.LogInformation(str);


            log.LogInformation(Environment.GetEnvironmentVariable("ConnStrBSC"));

            log.LogInformation(Environment.GetEnvironmentVariable("ConnStrETH"));


            if (getBlockIOPrivateKeys.Count() == 0) return new BadRequestObjectResult(new { error = "You must provide at least one private key to use GetBlock rpc provider" });


            var rpcProvider = new GetBlockIOProvider(getBlockIOPrivateKeys, network);

            var web3 = new Web3Geth(rpcProvider.GetNextRpcUrl());

            var tracer = new GethWeb3Tracer(web3);

            var indexer = new Indexer(
                tracer,
                log,
                    network == "bsc" ?
                    Environment.GetEnvironmentVariable("ConnStrBSC") :
                    Environment.GetEnvironmentVariable("ConnStrETH"),
                rpcProvider,
                string.IsNullOrEmpty(queueSize) ? 500 : Convert.ToInt32(queueSize)
            );

            var startBlock = !string.IsNullOrEmpty(startBlockP) ? ulong.Parse(startBlockP) : 0;

            var endBlock = !string.IsNullOrEmpty(endBlockP) ? ulong.Parse(endBlockP) : await indexer.GetLatestBlockNumber();

            await indexer.StartMonitorNewBlocks(2);

            return new OkResult();
        }
    }
}
