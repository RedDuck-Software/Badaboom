using Nethereum.Web3;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web3Tracer.Models;

namespace Web3Tracer.Tracers
{
    public interface IWeb3Tracer
    {
        Web3 Web3 { get; }

        Task<IEnumerable<TraceResult>> GetTracesForTransaction(string txHash);

        void ChangeWeb3Provider(string newRpcUrl);
    }
}
