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

        [HttpPost("GetTransactions"), AllowAnonymous]
        public async Task<ActionResult<PaginationTransactionResponse>> GetFilteredTransactions(
            [FromBody] GetFilteredTransactionRequest request)
        {
            if (request.DecodeInputDataInfo != null &&
                request.DecodeInputDataInfo.ArgumentsNamesValues != null &&
                request.DecodeInputDataInfo.ArgumentsNamesValues.Count > 0)

                return BadRequest(new {message = "To use pro functions, call another endpoint."});
            
            return await _transactionService.GetPaginatedFilteredTransactions(request);
        }

        [HttpGet("getInternalTxn")]
        public async Task<ActionResult<IEnumerable<InternalTxnResponse>>> GetInternalTxn([FromQuery] string txhash)
        {
            return Ok(await _transactionService.GetInternalTxns(txhash));
        }
    }
}