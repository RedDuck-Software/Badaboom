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
    public class PaymentController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;


        public PaymentController(IUserService userService, IPaymentService paymentService)
        {
            _userService = userService;
            _paymentService = paymentService;
        }

        [HttpPost("purchase")]
        [Badaboom.Backend.Attributes.Authorize]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            return BadRequest(new { message = "not implemeted" });
        }
    }
}
