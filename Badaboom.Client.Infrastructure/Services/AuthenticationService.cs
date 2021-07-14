using Badaboom.Client.Infrastructure.Models;
using Badaboom.Core.Models.Response;
using MetaMask.Blazor;
using MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Badaboom.Client.Infrastructure.Services
{
    public interface IAuthenticationService
    {
        User User { get; }
        Task Initialize();
        Task Connect(string address);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private MetaMaskService _metaMaskService;

        public User User { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            MetaMaskService metaMaskService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _metaMaskService = metaMaskService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<User>("user");
        }

        public async Task Connect(string address)
        {
            string userResponceNonce = default;

            var userExistsResponce = await _httpService.HttpClient.GetAsync($"/api/auth/user/{address}");
            if (userExistsResponce.StatusCode == HttpStatusCode.NotFound)
            {
                userResponceNonce = await Register(address);
            }
            else if (userExistsResponce.StatusCode == HttpStatusCode.OK)
            {
                string content = await userExistsResponce.Content.ReadAsStringAsync();

                UserResponce userResponce = JsonSerializer.Deserialize<UserResponce>(content,
                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                userResponceNonce = userResponce.Nonce;
            }
            else
            {
                Console.WriteLine("Error: StatusCode != HttpStatusCode.OK and StatusCode != HttpStatusCode.NotFound)");
            }

            string signedNonce = await SignData("Click 'Sign' to connect to the server", userResponceNonce);

            await Authenticate(address, signedNonce);

            await _localStorageService.SetItem("user", User);
        }

        /// <param name="address">MetaMask account address</param>
        /// <returns>User response nonce</returns>
        private async Task<string> Register(string address)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { address }), System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpService.HttpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            var userResponce = JsonSerializer.Deserialize<UserResponce>(content, 
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return userResponce.Nonce;
        }

        /// <param name="address">MetaMask account address</param>
        /// <param name="signedNonce">Signed nonce with key</param>
        private async Task Authenticate(string address, string signedNonce)
        {
            var userAuthRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/authenticate")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { address, signedNonce }), System.Text.Encoding.UTF8, "application/json")
            };
            var userAuthResponce = await _httpService.HttpClient.SendAsync(userAuthRequest);

            string content = await userAuthResponce.Content.ReadAsStringAsync();

            User = JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            User.Address = address;
        }

        private async Task<string> SignData(string label, string value)
        {
            string signData;
            try
            {
                var result = await _metaMaskService.SignTypedData(label, value);
                signData = result;
            }
            catch (UserDeniedException)
            {
                signData = "User Denied";
            }
            catch (Exception ex)
            {
                signData = $"Exception: {ex}";
            }

            Console.WriteLine(signData);
            return signData;
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}