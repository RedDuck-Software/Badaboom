using System.Threading.Tasks;
using Badaboom_indexer.Extensions;
using System;
using Web3Tracer.Tracers.Geth;
using Nethereum.Web3;
using Nethereum.Geth;

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
            var web3 = new Web3Geth(args[0]);

            var tracer = new GethWeb3Tracer(web3);

            var indexer = new Indexer(tracer);

            var startBlock = args.Length > 1 ? ulong.Parse(args[1]) : 0;

            var endBlock = args.Length > 2 ? ulong.Parse(args[2]) : await indexer.GetLatestBlockNumber();

            await indexer.IndexInRangeParallel(startBlock, endBlock, 20);

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");

            ConsoleColor.DarkMagenta.WriteLine("\n\nStarting getting new blocks...\n\n");

            await indexer.StartMonitorNewBlocks();
        }
    }
}
