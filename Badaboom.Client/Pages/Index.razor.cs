using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Badaboom.Core.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Badaboom.Client.Infrastructure.Services;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using MetaMask.Blazor;
using System.Numerics;

namespace Badaboom.Client.Pages
{
    public partial class Index
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public Microsoft.Extensions.Configuration.IConfiguration Config { get; set; }

        [Inject]
        public IHttpService _httpService { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }

        public int TotalPageQuantity { get; set; }

        public GetFilteredTransactionRequest TransactionFilter { get; set; } = new GetFilteredTransactionRequest();

        public bool Loading { get; set; }

        public long CallId { get; set; }

        int maximumPages = 1;


        protected override async Task OnInitializedAsync()
        {
            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
            Loading = true;

            if (TransactionFilter.MethodId != null)
                TransactionFilter.MethodId = ToValidHexString(TransactionFilter.MethodId);

            Transactions = null;

            PaginationTransactionResponse paginationTransactionResponse;

            if (TransactionFilter.DecodeInputDataInfo != null &&
                TransactionFilter.DecodeInputDataInfo.ArgumentsNamesValues != null &&
                TransactionFilter.DecodeInputDataInfo.ArgumentsNamesValues.Count > 0)
            {
                paginationTransactionResponse = await _httpService.Post<PaginationTransactionResponse>("/api/Transaction/getTransactionsByArgument", TransactionFilter);
            }
            else
            {
                HttpResponseMessage httpResponse = await Http.PostAsync("/api/Transaction/GetTransactions",
                    new StringContent(
                        JsonSerializer.Serialize(TransactionFilter),
                        Encoding.UTF8,
                        "application/json")
                    );

                var responseString = await httpResponse.Content.ReadAsStringAsync();

                paginationTransactionResponse = JsonSerializer.Deserialize<PaginationTransactionResponse>(responseString,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }

            System.Console.WriteLine(paginationTransactionResponse != null);

            if (paginationTransactionResponse != null)
            {
                Transactions = paginationTransactionResponse.Transactions;

                int totalCount = paginationTransactionResponse.Count;

                if (totalCount != 0)
                {
                    TotalPageQuantity = totalCount / TransactionFilter.Count + (totalCount % TransactionFilter.Count != 0 ? 1 : 0);
                }
                else
                {
                    if (Transactions.Count() == TransactionFilter.Count && TotalPageQuantity == TransactionFilter.Page)
                    {
                        maximumPages += 1;
                    }

                    TotalPageQuantity = maximumPages;
                }

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
            maximumPages = 1;
            TotalPageQuantity = 1;
            TransactionFilter.Page = 1;
            await LoadTransactions();
        }

    }
}
