﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ClientCore.Models;
using MetaMask.Blazor;
using MetaMask.Blazor.Enums;
using MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;
//using Nethereum.ABI.FunctionEncoding;
//using Nethereum.ABI.Model;

namespace Client.Pages
{
    public partial class Index
    {
        [Inject]
        public HttpClient Http { get; set; } = default!;

        public IEnumerable<Transaction> Transactions { get; set; }
        public int TotalPageQuantity { get; set; }
        public int CurrentPage { get; set; } = 1;


        protected override async Task OnInitializedAsync()
        {
            await LoadTransactions();
        }

        public async Task LoadTransactions(int page = 1, int quantityPerPage = 9)
        {
            var httpResponse = await Http.GetAsync($"https://localhost:44345/api/Transaction?page={page}&quantityPerPage={quantityPerPage}");

            if (httpResponse.IsSuccessStatusCode)
            {
                //totalPageQuantity = int.Parse(httpResponse.Headers.GetValues("pagesQuantity").FirstOrDefault());
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
            CurrentPage = page;
            await LoadTransactions(page);
        }
    }
}
