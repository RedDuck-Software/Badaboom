using System.Threading.Tasks;
using Database;
using IndexerCore;
using Nethereum.Geth;
using Web3Tracer.Tracers.Geth;

namespace Monitor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var web3 = new Web3Geth(args[1]);

            var tracer = new GethWeb3Tracer(web3);

            var indexer = new Indexer(
                tracer,
                ConnectionStrings.GetDefaultConnectionToDatabase(
                    args[0] == "bsc" ?
                    ConnectionStrings.BscDbName :
                    ConnectionStrings.EthDbName
                )
            );

            await indexer.StartMonitorNewBlocks();
        }
    }
}
