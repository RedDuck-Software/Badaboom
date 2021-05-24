using Nethereum.Web3;
using Database.Respositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using Badaboom_indexer.Extensions;
using System;
using Database.Models;

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
            var web3 = new Web3(args[0]);

            var indexer = new Indexer(web3);

            await indexer.IndexInRangeParallel(ulong.Parse(args[1]), ulong.Parse(args[2]));

            ConsoleColor.Magenta.WriteLine("\nIndexing successfully done!");
        }
    }
}
