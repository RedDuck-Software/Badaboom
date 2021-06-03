using Nethereum.Parity;
using System.Threading.Tasks;
using Badaboom_indexer.Extensions;
using System;

namespace Badaboom_indexer
{
    class Program
    {

        /// <summary>
        /// First args element must be web3 provider url
        /// Second - startBlock number
        /// Third - endBlock number
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var parity = new Web3Parity(args[0]);

            var indexer = new Indexer(parity);

            var startBlock = args.Length > 1 ? ulong.Parse(args[1]) : 0;

            var endBlock = args.Length > 2 ? ulong.Parse(args[2]) : await indexer.GetLatestBlockNumber();

            await indexer.IndexInRangeParallel(startBlock, endBlock, 10);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");

            ConsoleColor.DarkMagenta.WriteLine("Starting getting new blocks...\n\n");

            await indexer.StartMonitorNewBlocks();
        }
    }
}
