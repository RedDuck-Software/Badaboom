using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Badaboom.Backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("AllowAll")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;


        public PaymentController(
            IPaymentService paymentService
            )
        {
            _paymentService = paymentService;
        }

        [HttpGet("walletAddressToSend"), Attributes.Authorize]
        public async Task<IActionResult> GetAddressToSend()
        {
            string address = _paymentService.GetWalletAddress();
            return Ok(new { walletAddress = address });
        }

        [HttpPost("purchase"), Attributes.Authorize]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = $"The number of units of the product to purchase must be greater than 0" });
            }

            bool transactionIsValid = await _paymentService.ValidatePurchase(
                request.TxnHash, 
                CurrentUser.Address, 
                await _paymentService.PurchaseCost(ProductType.ArgumentFunctionRequests, request.Quantity));

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

        [HttpPost("checkPossibilityUsingFunction"), Attributes.Authorize]
        public async Task<IActionResult> CheckPossibilityUsingFunction([FromBody] ProductRequest request)
        {
            int? quantity = await _paymentService.CheckQuantity(request.ProductType, CurrentUser.Address);
            return Ok(new { Quantity = quantity ?? 0 });
        }

        [HttpGet("productPrice"), AllowAnonymous]
        public async Task<ActionResult<ProductPriceResponse>> GetProductPrice([FromQuery] ProductRequest request)
        {
            var productPrice = await _paymentService.GetProductPrice(request.ProductType);

            return Ok(productPrice);
        }
    }
}
