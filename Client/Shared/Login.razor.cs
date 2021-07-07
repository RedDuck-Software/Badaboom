using MetaMask.Blazor;
using MetaMask.Blazor.Enums;
using MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;


namespace Client.Shared
{
    public partial class Login : IDisposable
    {
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default!;

        public bool HasMetaMask { get; set; }
        public string SelectedAddress { get; set; }
        public string SelectedChain { get; set; }
        public string TransactionCount { get; set; }
        public string SignedData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            #region Metamask

            //Subscribe to events
            MetaMaskService.AccountChangedEvent += MetaMaskService_AccountChangedEvent;

            HasMetaMask = await MetaMaskService.HasMetaMask();
            if (HasMetaMask)
            {
                await MetaMaskService.ListenToEvents();
            }

            bool isSiteConnected = await MetaMaskService.IsSiteConnected();
            if (isSiteConnected)
            {
                await GetSelectedAddress();
            }

            #endregion
        }

        #region Metamask

        private async Task MetaMaskService_AccountChangedEvent(string arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        public async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();
            await GetSelectedAddress();
        }

        public async Task GetSelectedAddress()
        {
            SelectedAddress = await MetaMaskService.GetSelectedAddress();
            Console.WriteLine($"Address: {SelectedAddress}");
        }

        public async Task SignData(string label, string value)
        {
            try
            {
                var result = await MetaMaskService.SignTypedData(label, value);
                SignedData = $"Signed: {result}";
            }
            catch (UserDeniedException)
            {
                SignedData = "User Denied";
            }
            catch (Exception ex)
            {
                SignedData = $"Exception: {ex}";
            }
        }

        #endregion

        public void Dispose()
        {
            MetaMaskService.AccountChangedEvent -= MetaMaskService_AccountChangedEvent;
        }

    }
}
