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

        public PurchaseAttribute(
            ProductType endpoint
            )
        {
            this.endpoint = endpoint;
        }

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
                var paymentService = context.HttpContext.RequestServices.GetService(typeof(IPaymentService)) as PaymentService;

                await paymentService.SetProduct(user.Address, ProductType.ArgumentFunctionRequests, -1);
            }
        }
    }
}
