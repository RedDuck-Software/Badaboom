using BackendCore.Services;
using Badaboom.Backend.Infrastructure.Services;
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
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;


        public PaymentController(IUserService userService, IPaymentService paymentService)
        {
            _userService = userService;
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
            //decimal cost = _paymentService.PurchaseCost(request.ProductType, request.Quantity);
            decimal costTest = 0.01m;



            return BadRequest(new { message = "not implemeted" });
        }
    }
}
