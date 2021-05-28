using Nethereum.Web3;
using Nethereum.Parity;
using Nethereum.Geth;

using System.Threading.Tasks;
using Badaboom_indexer.Extensions;
using System;
using System.Net;
using Nethereum.Parity.RPC.Trace;
using Newtonsoft.Json;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Badaboom_indexer.Models;

namespace Badaboom_indexer
{
    class Program
    {

        /// <summary>
        /// First args element must be web3 provider url
        /// Second - startBlock
        /// Third - endBlock
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var parity = new Web3Parity(args[0]);

      
            var indexer = await Indexer.CreateInstance(parity);

            var startBlock = args.Length > 1 ? ulong.Parse(args[1]) : 0;

            var endBlock = args.Length > 2 ? ulong.Parse(args[2]) : indexer.LatestBlockNumber;

            await indexer.IndexInRangeParallel(1000000, endBlock, 1);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");
        }
    }
}
