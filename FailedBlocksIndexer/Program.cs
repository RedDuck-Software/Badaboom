using Database;
using IndexerCore;
using Nethereum.Geth;
using System.Threading.Tasks;
using Web3Tracer.Tracers.Geth;

namespace FailedBlocksIndexer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var web3 = new Web3Geth(args[1]);

            var tracer = new GethWeb3Tracer(web3);

            var indexer = new Indexer(
                 tracer,
                     args[0] == "bsc" ?
                     ConnectionStrings.GetInstance().BscDbName :
                     ConnectionStrings.GetInstance().EthDbName
             );

            await indexer.IndexFailedAndPendingBlocks();
        }
    }
}
