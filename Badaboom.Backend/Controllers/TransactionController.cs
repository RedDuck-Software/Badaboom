using Badaboom.Backend.Controllers;
using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Enums;
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

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;
        private readonly IPaymentService _paymentService;

        public TransactionController(
            ITransactionService transactionSerivice,
            IPaymentService paymentService
            )
        {
            _transactionService = transactionSerivice;
            _paymentService = paymentService;
        }


        [HttpGet("GetContractAbi")]
        public async Task<ActionResult<string>> GetContractAbi([FromQuery] string contractAddress)
        {
            var res = await _transactionService.GetContractAbi(contractAddress);

            if (res == null) return new NotFoundResult();

            return res;
        }

        [HttpPost("GetTransactions")]
        public async Task<ActionResult<PaginationTransactionResponse>> GetFilteredTransactions([FromBody] GetFilteredTransactionRequest request)
        {
            if (request.DecodeInputDataInfo != null &&
                request.DecodeInputDataInfo.ArgumentsNamesValues != null && 
                request.DecodeInputDataInfo.ArgumentsNamesValues.Count > 0)
            {
                return BadRequest(new { message = "To use pro functions, call another endpoint." });
            }
            else
            {
                return await _transactionService.GetPaginatedFilteredTransactions(request);
            }
        }

        [HttpPost("getTransactionsByArgument")]
        [Badaboom.Backend.Attributes.Authorize]
        public async Task<ActionResult<PaginationTransactionResponse>> GetFilteredTransactionsByArgument([FromBody] GetFilteredTransactionRequest request)
        {
            int? quantity = CurrentUser.AvailableProduct[ProductType.ArgumentFunctionRequests.ToString()];

            if (quantity == null)
            {
                return BadRequest(new { message = "Not enough requests to filter" });
            }

            await _paymentService.SetProduct(CurrentUser.Address, ProductType.ArgumentFunctionRequests, -1);
            return await _transactionService.GetPaginatedFilteredTransactionsWithInputParameters(request, 10);
        }
    }
}
