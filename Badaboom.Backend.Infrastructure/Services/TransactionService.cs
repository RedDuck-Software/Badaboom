using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Database.Models;
using Database.Respositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface ITransactionService
    {
        Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request);
    }

    public class TransactionService : ITransactionService
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TransactionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request)
        {
            IEnumerable<Call> res;

            using (var tRepo = new TransactionRepository(_connectionString))
            {
                res =  await tRepo.GetCallsByAddressAndMethodAsync(request.ContractAddress, request.MethodId);
            }



        }
    }
}
