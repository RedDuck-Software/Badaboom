using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Badaboom.Core.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Badaboom.Client.Pages
{
    public partial class Index
    {
        [Inject]
        public HttpClient Http { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }

        public int TotalPageQuantity { get; set; }

        public GetFilteredTransactionRequest TransactionFilter { get; set; } = new GetFilteredTransactionRequest();

        public bool Loading { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
            Console.WriteLine(JsonSerializer.Serialize(TransactionFilter));

            if(TransactionFilter.MethodId != null)
                TransactionFilter.MethodId = ToValidHexString(TransactionFilter.MethodId);

            Loading = true;

            Transactions = null;

            var httpResponse = await Http.PostAsync("/api/Transaction/GetTransactions",
                new StringContent(
                    JsonSerializer.Serialize(TransactionFilter),
                    Encoding.UTF8,
                    "application/json")
                );

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                var paginationTransactionResponse = JsonSerializer.Deserialize<PaginationTransactionResponse>(responseString,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                TotalPageQuantity = paginationTransactionResponse.Count;
                Transactions = paginationTransactionResponse.Transactions;
                StateHasChanged();
            }
            else
            {
                // handle error
            }

            Loading = false;
        }
        private string ToValidHexString(string value)
        {
            if (value.StartsWith("0x"))
            {
                return value;
            }

            return HashingService.EncodeMethodSignature(value.Replace(" ", ""));
        }

        private async Task SelectedPage(int page)
        {
            TransactionFilter.Page = page;
            await LoadTransactions();
        }

        public async Task Filter()
        {
            Console.WriteLine(JsonSerializer.Serialize(TransactionFilter));
            TransactionFilter.Page = 1;
            await LoadTransactions();
        }
    }
}
