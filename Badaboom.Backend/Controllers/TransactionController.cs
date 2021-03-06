using Badaboom.Backend.Controllers;
using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Badaboom.Backend.Attributes;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

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


        [HttpGet("GetContractAbi"), AllowAnonymous]
        public async Task<ActionResult<string>> GetContractAbi([FromQuery] string contractAddress)
        {
            var res = await _transactionService.GetContractAbi(contractAddress);

            if (res == null) return new NotFoundResult();

            return res;
        }

        [HttpPost("GetTransactions"), AllowAnonymous]
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

        [HttpPost("getTransactionsByArgument"), Badaboom.Backend.Attributes.Authorize]
        [Purchase(ProductType.ArgumentFunctionRequests)]
        public async Task<ActionResult<PaginationTransactionResponse>> GetFilteredTransactionsByArgument([FromBody] GetFilteredTransactionRequest request)
        {
            return await _transactionService.GetPaginatedFilteredTransactionsWithInputParameters(request, 10);
        }

        [HttpGet("getInternalTxn")]
        public async Task<ActionResult<IEnumerable<InternalTxnResponse>>> GetInternalTxn([FromQuery] string txhash)
        {
            return Ok(await _transactionService.GetInternalTxns(txhash));
        }
    }
}
