using BackendCore.Services;
using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Request;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badaboom.Backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("AllowAll")]
    [Badaboom.Backend.Attributes.Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;


        public PaymentController(
            IPaymentService paymentService
            )
        {
            _paymentService = paymentService;
        }

        [HttpGet("walletAddressToSend")]
        public async Task<IActionResult> GetAddressToSend()
        {
            string address = _paymentService.GetWalletAddress();
            return Ok(new { walletAddress = address });
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            bool transactionIsValid = await _paymentService.ValidatePurchase(request.TxnHash, CurrentUser.Address, 0.1m); //hardcode - shoud get price from method

            if (transactionIsValid)
            {
                await _paymentService.SetProduct(CurrentUser.Address, ProductType.ArgumentFunctionRequests, request.Quantity);
                return Ok(new { message = $"{request.Quantity} requests added successfully." });
            }
            else
            {
                return BadRequest(new { message = "You sent incorrect data." });
            }
        }

        [HttpPost("checkPossibilityUsingFunction")]
        public async Task<IActionResult> CheckPossibilityUsingFunction([FromBody] ProductRequest request)
        {
            int quantity = await _paymentService.CheckQuantity(request.ProductType, CurrentUser.Address);
            return Ok(new { Quantity = quantity });
        }

        [HttpPost("useProduct")]
        public async Task<IActionResult> UseProduct([FromBody] ProductRequest request) // use only after "CheckPossibilityUsingFunction" 
        {
            await _paymentService.SetProduct(CurrentUser.Address, request.ProductType, -1);
            return Ok();
        }
    }
}
