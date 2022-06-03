using Badaboom.Client.Infrastructure.Services;
using Badaboom.Client.Shared;
using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Badaboom.Client.Pages.Pricing.Product
{
    public partial class Product
    {
        private int quantityForBuy = 1;

        private bool loading = false;

        [Parameter]
        public ProductType ProductType { get; set; }

        [Parameter]
        public ProductPriceResponse ProductPrice { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private int CurrentDiscount { get; set; }

        private BigInteger TotalPrice { get; set; }

        private IOrderedEnumerable<KeyValuePair<int, byte>> SortedDiscounts { get; set; }

        private int QuantityForBuy
        {
            get => quantityForBuy;

            set
            {
                quantityForBuy = value > 1000 ? 1000 : value < 1 ? 1 : value;
                CountTotalPrice();
                StateHasChanged();
            }
        }

        public Modal Modal { get; set; }


        protected override async Task OnInitializedAsync()
        {
            TotalPrice = ProductPrice.PricePerItem;
            SortedDiscounts = ProductPrice.AmountFromPercents.OrderByDescending(x => x.Key);
            StateHasChanged();
        }

        protected void CountTotalPrice()
        {
            foreach (var item in SortedDiscounts)
            {
                if (QuantityForBuy >= item.Key)
                {
                    TotalPrice = (BigInteger)ProductPrice.PricePerItem * QuantityForBuy * item.Value / 100;

                    CurrentDiscount = 100 - item.Value;

                    return;
                }
            }

            CurrentDiscount = 0;

            TotalPrice = ProductPrice.PricePerItem * QuantityForBuy;
        }

        [Inject]
        private IHttpService _httpService { get; set; }

        [Inject]
        private IConfiguration _config { get; set; }

        [Inject]
        private IAuthenticationService _authService { get; set; }

        [Inject]
        private ILocalStorageService _localStorageService { get; set; }

        [Inject]
        public MetaMask.Blazor.MetaMaskService MetaMaskService { get; set; } = default!;

        protected async Task Buy()
        {
            loading = true;

            StateHasChanged();

            
            string toAddress = (await _httpService.Get<WalletAddress>("/api/Payment/walletAddressToSend"))?.walletAddress; // generate ex if auth token ends

            if (toAddress is null)
            {
                System.Console.WriteLine("User unauthorized");
                NavigationManager.NavigateTo("logout");
                return;
            }

            string txnHash;

            try
            {
                txnHash = await MetaMaskService.SendTransaction(toAddress, TotalPrice);

                if (await CheckIsSuccessTransaction(txnHash))
                {
                    System.Console.WriteLine("(Transaction status code success)");

                    await _httpService.Post<PurchaseRequest>("/api/payment/purchase", new PurchaseRequest()
                    {
                        ProductType = ProductType,
                        Quantity = QuantityForBuy,
                        TxnHash = txnHash
                    });

                    if (_authService.User.AvailableProduct.ContainsKey(ProductType.ToString()))
                        _authService.User.AvailableProduct[ProductType.ToString()] += QuantityForBuy;
                    else
                        _authService.User.AvailableProduct.Add(ProductType.ToString(), QuantityForBuy);

                    await _localStorageService.SetItem("user", _authService.User);
                }
                else
                {
                    System.Console.WriteLine("(There is something wrong with your transaction)");
                    loading = false;
                }
            }
            catch (MetaMask.Blazor.Exceptions.UserDeniedException)
            {
                System.Console.WriteLine($"User Denied");
                loading = false;
            }
        }

        private async Task<bool> CheckIsSuccessTransaction(string txnHash)
        {
            int cycle = 0, maxCycle = 18 * 2;
            Nethereum.Web3.Web3 web3 = new(_config["RPCUrl"]);

            System.Console.WriteLine($"Wait between 5 and 180 seconds, we check your transaction");
            
            while (cycle < maxCycle)
            {
                var transaction = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txnHash);

                await Task.Delay(5_000);

                try
                {
                    if (transaction.Status.Value == 1)
                    {
                        return true;
                    }
                }
                catch (System.NullReferenceException)
                {
                    System.Console.WriteLine("(Pending)");
                }

                cycle++;
            }

            return false;
        }

        private class WalletAddress
        {
            public string walletAddress { get; set; }
        }

        private class ResponseQuantity
        {
            public int? Quantity { get; set; }
        }
    }
}