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

            string contract = "0x110379fc7B83Ff04B22D440C35392d0d7735DD63";
            ulong blockNumber = 10319428;

            /* var res = await parity.Eth.GetStorageAt.SendRequestAsync(
                 contract,
                 new HexBigInteger("0"),
                 new BlockParameter(blockNumber));

             Console.WriteLine(res);

             res = await parity.Eth.GetStorageAt.SendRequestAsync(
                 contract,
                 new HexBigInteger("1"),
                 new BlockParameter(blockNumber));
             Console.WriteLine(res);


             res = await parity.Eth.GetStorageAt.SendRequestAsync(
                 contract,
                 new HexBigInteger("2"),
                 new BlockParameter(blockNumber));
             Console.WriteLine(res);

             res = await parity.Eth.GetStorageAt.SendRequestAsync(
                 contract,
                 new HexBigInteger("3"),
                 new BlockParameter(blockNumber));
             Console.WriteLine(res);

             res = await parity.Eth.GetStorageAt.SendRequestAsync(
                 contract,
                 new HexBigInteger("4"),
                 new BlockParameter(blockNumber));
             Console.WriteLine(res);

             res = await parity.Eth.GetStorageAt.SendRequestAsync(
                contract,
                new HexBigInteger("5"),
                new BlockParameter(blockNumber));
             Console.WriteLine(res);
 */


            //var res = await parity.Trace.TraceTransaction.SendRequestAsync("0x9e12e5a13e9dcceb1cc949494ccbf5391616556ff1e730a467aaa7cc80238615");

            var indexer = await Indexer.CreateInstance(parity);

            var startBlock = args.Length > 1 ? ulong.Parse(args[1]) : 0;

            var endBlock = args.Length > 2 ? ulong.Parse(args[2]) : indexer.LatestBlockNumber;

            await indexer.IndexInRangeParallel(50000, endBlock, 1);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");
        }
    }
}
