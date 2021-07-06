using Client.Helpers;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
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

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        //private readonly ApplicationDbContext context;

        //public TransactionController(ApplicationDbContext context)
        //{
        //    this.context = context;
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult<List<Transaction>>> Get([FromQuery] PaginationDTO pagination,
        //    [FromQuery] string name)
        //{
        //    var queryable = context.People.AsQueryable();
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        queryable = queryable.Where(x => x.Name.Contains(name));
        //    }
        //    await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
        //    return await queryable.Paginate(pagination).ToListAsync();
        //}

        [HttpGet/*(Name = "transactions")*/]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Transaction>>> Get([FromQuery] PaginationDTO pagination)
        {
            var transactions = JsonConvert.DeserializeObject<IEnumerable<Transaction>>("sample-data/temp-result.json");

            await HttpContext.InsertPaginationParameterInResponse(transactions, pagination.QuantityPerPage);

            transactions = transactions
                .Skip((pagination.Page - 1) * pagination.QuantityPerPage)
                .Take(pagination.QuantityPerPage);

            return transactions.ToList();
        }

    }
}
