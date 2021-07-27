using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Enums;
using Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Badaboom.Backend.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PurchaseAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly ProductType endpoint;
        //private readonly IPaymentService _paymentService;

        public PurchaseAttribute(
            ProductType endpoint //,
            //IPaymentService paymentService
            )
        {
            this.endpoint = endpoint;
            //_paymentService = paymentService;
        }

        [Inject]
        public IPaymentService PaymentService { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];

            if (!user.AvailableProduct.ContainsKey(endpoint.ToString()))
            {
                context.Result = new JsonResult(new { message = "Not enough requests for using pro function. pls buy requests before use this endpoint" }) 
                { 
                    StatusCode = StatusCodes.Status405MethodNotAllowed 
                };
            }
            else
            {
                await PaymentService.SetProduct(user.Address, ProductType.ArgumentFunctionRequests, -1);
            }
        }
    }
}
