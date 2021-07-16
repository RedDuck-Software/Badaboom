using Badaboom.Client.Shared.DTOs;
using Badaboom.Core.Models.DTOs;
using Badaboom.Core.Models.Request;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System.Text;

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

        public Dictionary<Input, string> SelectedArgumentsValues { get; set; } = new(new AbiInputComparer());

        [Inject]
        public HttpClient Http { get; set; }


        protected void RemoveSelectedArgument(Input arg)
        {
            SelectedArgumentsValues.Remove(arg);
            SelectedMethodAvailableArguments.Add(arg);
            StateHasChanged();
            UpdateFilteredTranscations();
        }

        protected void OnMethodSelected(Method selection)
        {
            SelectedMethod = selection;
            SelectedMethodAvailableArguments = selection.Inputs.ToList();
            SelectedArgumentsValues.Clear();
            StateHasChanged();
            UpdateFilteredTranscations();
        }

        protected void OnArgumentSelected(Input input)
        {
            SelectedArgumentsValues.Add(input, null);
            SelectedMethodAvailableArguments.Remove(input);
            StateHasChanged();
            UpdateFilteredTranscations();
        }

        protected void UpdateFilteredTranscations()
        {
            if (SelectedMethod != null)
                TransactionFilters.MethodId = $"{SelectedMethod.Name}({string.Join(",", SelectedMethod.Inputs.Select(v => v.Type))})";

            Console.WriteLine(TransactionFilters.MethodId);

            TransactionFilters.DecodeInputDataInfo = SelectedMethod == null ? null : new()
            {
                FunctionAbis = new Method[] { SelectedMethod },
                ArgumentsNamesValues = SelectedArgumentsValues.Count == 0 ? null :
                    new Dictionary<string, string>(
                        SelectedArgumentsValues.Select(vp =>
                            new KeyValuePair<string, string>(vp.Key.Name, vp.Value)))
            };

            Console.WriteLine(JsonSerializer.Serialize(TransactionFilters));
        }
    }
}
