using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Badaboom.Client.Pages.Pricing
{
    public partial class Pricing
    {
        private bool loading = true;

        [Inject]
        public HttpClient Http { get; set; }

        public List<(ProductType Product, ProductPriceResponse ProductPrice)> ProductsPrice { get; set; } = new();


        protected override async Task OnInitializedAsync()
        {
            foreach (ProductType product in Enum.GetValues<ProductType>())
            {
                var httpResponse = await Http.GetAsync($"/api/Payment/productPrice?ProductType={product}");

                var responseString = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine(responseString);

                ProductsPrice.Add((product, JsonSerializer.Deserialize<ProductPriceResponse>(responseString,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })));
            }

            loading = false;
        }
    }
}