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

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface ITransactionService
    {
        Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request);
    }

    public class TransactionService : ITransactionService
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionStringIndexes;
        private readonly string _connectionString;


        public TransactionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionStringIndexes = configuration.GetConnectionString("ETHIndexes");
        }

        public async Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request)
        {
            IEnumerable<Call> res;

            using (var tRepo = new TransactionRepository(_connectionStringIndexes))
            {
                res = await tRepo.GetCallsByAddressAndMethodAsync(new CallsPagination()
                {
                    BlockId = request.BlockNumber,
                    MethodId = request.MethodId,
                    To = request.ContractAddress,
                    Page = request.Page,
                    Count = request.Count
                });
            }

            return new PaginationTransactionResponse()
            {
                Count = res.Count(),

                Transactions = res.Select(c =>
                    {
                        return new Core.Models.Response.Transaction()
                        {
                            Method = c.MethodId,
                            Block = (ulong)c.BlockId,
                            From = c.From,
                            To = c.To,
                            TxnHash = c.TransactionHash,
                            Age = (ulong)c.TimeStamp
                        };
                    }
                )
            };
        }
    }
}
