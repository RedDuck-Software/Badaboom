using Badaboom.Backend.Controllers;
using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class TransactionController : BaseController
    {
        //private readonly ApplicationDbContext context;

        //public TransactionController(ApplicationDbContext context)
        //{
        //    this.context = context;
        //}
        private ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionSerivice)
        {
            _transactionService = transactionSerivice;
        }



        [HttpGet("GetContractAbi")]
        public async Task<ActionResult<string>> GetContractAbi([FromQuery] string contractAddress)
        {
            var res = await _transactionService.GetContractAbi(contractAddress);

            if (res == null) return new NotFoundResult();

            return res;
        }

        [HttpGet("GetTransactions")]
        public async Task<ActionResult<PaginationTransactionResponse>> GetFilteredTransactions([FromQuery] GetFilteredTransactionRequest request)
        {
            IEnumerable<Transaction> res;

            if (request.DecodeInputDataInfo != null)
                res = await _transactionService.GetPaginatedFilteredTransactionsWithInputParameters(request, 10);
            else
                res = await _transactionService.GetPaginatedFilteredTransactions(request);

            return new PaginationTransactionResponse()
            {
                Count = res.Count(),
                Transactions = res.ToList()
            };
        }

        private static int CountPageQuantity(IEnumerable<Transaction> enumerable, int recordsPerPage)
        {
            int count = enumerable.Count();
            int additionalPage = count % recordsPerPage;
            int pagesQuantity = count / recordsPerPage + (additionalPage > 0 ? 1 : 0);
            return pagesQuantity;
        }
    }
}
