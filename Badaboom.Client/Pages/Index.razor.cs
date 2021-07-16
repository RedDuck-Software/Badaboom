using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Badaboom.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
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

        protected override async Task OnInitializedAsync()
        {
            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
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
        }
        private string ToValidHexString(string value)
        {
            if (value.StartsWith("0x")) return value;

            return HashingService.EncodeMethodSignature(value.Replace(" ", ""));
        }

        private async Task SelectedPage(int page)
        {
            TransactionFilter.Page = page;
            await LoadTransactions();
        }

        public async Task Filter()
        {
            TransactionFilter.Page = 1;
            await LoadTransactions();
        }

        public async Task Delete()
        {
            TransactionFilter.Page = 1;
            await LoadTransactions();
        }
    }
}
