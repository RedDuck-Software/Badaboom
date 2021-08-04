using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Badaboom.Client.Pages.Pricing.Product
{
    public partial class Product
    {
        private short quantityForBuy;

        [Parameter]
        public ProductType ProductType { get; set; }

        [Parameter]
        public ProductPriceResponse ProductPrice { get; set; }

        private int CurrentDiscount { get; set; }

        private BigInteger TotalPrice { get; set; }

        private IOrderedEnumerable<KeyValuePair<int, byte>> SortedDiscounts { get; set; }

        private short QuantityForBuy
        {
            get => quantityForBuy;

            set
            {
                quantityForBuy = value;
                CountTotalPrice();
                StateHasChanged();
            }
        }


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
    }
}