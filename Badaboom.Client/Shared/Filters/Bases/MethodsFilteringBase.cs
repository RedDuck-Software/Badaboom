using Badaboom.Core.Models.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace Badaboom.Client.Shared.Filters
{
    public class MethodsFilteringBase : ComponentBase
    {
        [Parameter]
        public string ContractAbiJSON { get; set; }

        public ContactAbiDTO ContractAbi { get; set; } 

        [Inject]
        public HttpClient Http { get; set; }
            

        protected override async Task OnInitializedAsync()
        {

        }
    }
}
