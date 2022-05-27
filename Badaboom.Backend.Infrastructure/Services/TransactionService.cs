using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Database.Models;
using Database.Respositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Model;
using Nethereum.Web3;
using Newtonsoft.Json;
using System.Net.Http;

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface ITransactionService
    {
        Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request, bool isCountCalculatedNeded = true);
        Task<IEnumerable<InternalTxnResponse>> GetInternalTxns(string txhash);
    }

    class EtherscanResponse
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public string Result { get; set; }
    }

    public class TransactionService : ITransactionService
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionStringIndexes;
        private readonly string _connectionString;

        private HttpClient HttpClient { get; set; }

        public TransactionService(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionStringIndexes = configuration.GetConnectionString("CrimeChainIndexes");
            HttpClient = client;
        }
        public async Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request, bool isCountCalculatedNeded = true)
        {
            (IEnumerable<Call> transactions, int? totalCount) res;

            using (var tRepo = new TransactionRepository(_connectionStringIndexes))
            {
                res = await tRepo.GetCalls(new CallsPagination()
                {
                    BlockId = request.BlockNumber,
                    MethodId = request.MethodId,
                    From = request.From,
                    To = request.ContractAddress,
                    Page = request.Page,
                    Count = request.Count,
                    CallIdFrom = request.CallId
                }, isCountCalculatedNeded);
            }

            return new PaginationTransactionResponse()
            {
                Count = res.totalCount ?? 0,
                Transactions = res.transactions.Select(c =>
                {
                    return new Core.Models.Response.Transaction()
                    {
                        Method = c.MethodId,
                        Block = (ulong)c.BlockId,
                        From = c.From,
                        To = c.To,
                        TxnHash = c.TransactionHash,
                        Age = (ulong)c.TimeStamp,
                        CallId = c.CallId
                    };
                })
            };
        }

        public async Task<IEnumerable<InternalTxnResponse>> GetInternalTxns(string txhash)
        {
            IEnumerable<Call> res;
            using (var tRepo = new TransactionRepository(_connectionStringIndexes))
            {
                res = await tRepo.GetInternalTransactions(txhash);
            };

            return res.Select(c => 
            {
                return new InternalTxnResponse()
                {
                    Type = (Core.Models.Enums.CallTypes)c.Type,
                    From = c.From,
                    To = c.To
                };
            });
        }
    }
}
