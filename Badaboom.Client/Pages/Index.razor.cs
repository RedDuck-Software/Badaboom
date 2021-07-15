using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
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
        public HttpClient Http { get; set; } = default!;

        public IEnumerable<Transaction> Transactions { get; set; }
        public int TotalPageQuantity { get; set; }
        public GetFilteredTransactionRequest TransactionFilter { get; set; } = new GetFilteredTransactionRequest();

        protected override async Task OnInitializedAsync()
        {
            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
            StringBuilder url = new($"/Transaction/GetTransactions?Count={TransactionFilter.Count}&Page={TransactionFilter.Page}");

            if (TransactionFilter.BlockNumber != null)
            {
                url.Append($"&BlockNumber={TransactionFilter.BlockNumber}");
            }
            if (!string.IsNullOrEmpty(TransactionFilter.ContractAddress))
            {
                url.Append($"&ContractAddress={TransactionFilter.ContractAddress}");
            }
            if (!string.IsNullOrEmpty(TransactionFilter.MethodId))
            {
                url.Append($"&MethodId={TransactionFilter.MethodId}");
            }
            Console.WriteLine(url.ToString());
            var httpResponse = await Http.GetAsync(url.ToString());

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                var paginationTransactionResponse = JsonSerializer.Deserialize<PaginationTransactionResponse>(responseString,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                TotalPageQuantity = paginationTransactionResponse.Count;
                Transactions = paginationTransactionResponse.Transactions;
            }
            else
            {
                // handle error
            }
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
