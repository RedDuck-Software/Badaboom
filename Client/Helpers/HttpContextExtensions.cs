using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task InsertPaginationParameterInResponse<T>(this HttpContext httpContext, 
            /*IQueryable<T> queryable*/IEnumerable<T> enumerable, 
            int recordsPerPage)
        {
            int additionalPage = enumerable.Count() % recordsPerPage;
            int pagesQuantity = (enumerable.Count() / recordsPerPage) + additionalPage > 0 ? 1 : 0;
            httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
    }
}
