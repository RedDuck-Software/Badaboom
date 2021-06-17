using System.Threading.Tasks;
using Database;
using IndexerCore;
using Nethereum.Geth;
using Web3Tracer.Tracers.Geth;

namespace Monitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var web3 = new Web3Geth(args[0]);

            var tracer = new GethWeb3Tracer(web3);

            var indexer = new Indexer(tracer, ConnectionStrings.GetDefaultConnectionToDatabase(ConnectionStrings.BscDbName));

            await indexer.StartMonitorNewBlocks();
        }
    }
}
