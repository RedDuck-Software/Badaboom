using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badaboom.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Badaboom.Core.Models.Response;
using System.Net.Http;
using System.Net;
using System.Text.Json;

namespace Badaboom.Client.Infrastructure.Services
{
    public interface IAuthenticationService
    {
        User User { get; }
        Task Initialize();
        Task Login(string address);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public User User { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<User>("user");
        }

        public async Task Login(string address)
        {
            UserResponce userResponce = null;

            var userExistsResponce = await _httpService.HttpClient.GetAsync($"/api/auth/user/{address}");
            if (userExistsResponce.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("new addres: " + address);

                var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register")
                {
                    Content = new StringContent(JsonSerializer.Serialize(new { address }), System.Text.Encoding.UTF8, "application/json")
                };

                var responce = await _httpService.HttpClient.SendAsync(request);
                string content = await responce.Content.ReadAsStringAsync();
                userResponce = JsonSerializer.Deserialize<UserResponce>(content);
            }
            else if (userExistsResponce.StatusCode == HttpStatusCode.OK)
            {
                string content = await userExistsResponce.Content.ReadAsStringAsync();
                userResponce = JsonSerializer.Deserialize<UserResponce>(content);
                //Console.WriteLine("OK: content: " + content);
            }
            else
            {
                Console.WriteLine("Error: StatusCode != HttpStatusCode.OK and StatusCode != HttpStatusCode.NotFound)");
            }

            //var userAuthRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/authenticate")
            //{
            //    Content = new StringContent(JsonSerializer.Serialize(new { address, userResponce.nonce }), System.Text.Encoding.UTF8, "application/json")
            //};
            //var userAuthResponce = await _httpService.HttpClient.SendAsync(userAuthRequest);

            //string body = await userAuthResponce.Content.ReadAsStringAsync();
            //Console.WriteLine("body: " + body);
            //User = JsonSerializer.Deserialize<User>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
            //User.Address = address;

            //Console.WriteLine($"UserId: {User.UserId}, AuthToken: {User.AuthToken}, RefreshToken: {User.RefreshToken}, Address: {User.Address}");

            await _localStorageService.SetItem("user", User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}