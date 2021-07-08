using MetaMask.Blazor;
using MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Badaboom.Client.Infrastructure.Services;
using Badaboom.Client.Infrastructure.Helpers;
using Badaboom.Client.Infrastructure.Models;

namespace Badaboom.Client.Pages
{
    public partial class Login : IDisposable
    {
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default!;
        public bool HasMetaMask { get; set; }
        public string SelectedAddress { get; set; }
        public string SignedData { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        public bool Loading { get; set; }

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

            // redirect to home if already logged in
            if (AuthenticationService.User != null)
            {
                NavigationManager.NavigateTo("/");
            }
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

        private async void LoginOnServer()
        {
            string selectedAddress = SelectedAddress;

            Loading = true;
            try
            {
                await AuthenticationService.Login(selectedAddress);
                var returnUrl = NavigationManager.QueryString("returnUrl") ?? "";
                NavigationManager.NavigateTo(returnUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Loading = false;
            }
        }

        public void Dispose()
        {
            MetaMaskService.AccountChangedEvent -= MetaMaskService_AccountChangedEvent;
        }

    }
}
