using Badaboom.Client.Shared.DTOs;
using Badaboom.Core.Models.DTOs;
using Badaboom.Core.Models.Request;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Badaboom.Client.Shared.Filters
{
    public class MethodsFilteringBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<Method> ContractMethods { get; set; }

        [Parameter]
        public GetFilteredTransactionRequest TransactionFilters { get; set; }

        [Parameter]
        public SelectedFiltersDTO SelectedFilters { get; set; }

        [Parameter]
        public Pages.Index Index { get; set; }

        public Method SelectedMethod { get; set; }

        public List<Input> SelectedMethodAvailableArguments { get; set; } = new();

        public List<Input> SelectedArguments { get; set; } = new();

        [Inject]
        public HttpClient Http { get; set; }


        protected void RemoveSelectedArgument(Input arg)
        {
            SelectedArguments.Remove(arg);
            SelectedMethodAvailableArguments.Add(arg);
            StateHasChanged();
        }

        protected void OnSelected(Method selection)
        {
            SelectedMethod = selection;
            SelectedMethodAvailableArguments = selection.Inputs.ToList();
            StateHasChanged();
        }

        protected void OnArgumentSelected(Input input)
        {
            Console.WriteLine(SelectedMethod);
            SelectedArguments.Add(input);
            SelectedMethodAvailableArguments.Remove(input);
            StateHasChanged();
        }
    }
}
