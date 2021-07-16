using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Badaboom.Client.Shared.Filters
{
    public partial class MethodsFilteringBase 
    {
        [Parameter]
        public string ContractAbi { get; set; }

        [Inject]
        public HttpClient Http { get; set; }
            
        

        protected override async Task OnInitializedAsync()
        {

        }
    }
}
