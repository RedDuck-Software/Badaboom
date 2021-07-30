using System.Collections.Generic;

namespace Badaboom.Core.Models.Response
{
    public class ProductPriceResponse
    {
        public long PricePerItem { get; set; } = 0;

        public Dictionary<int, byte> AmountFromPercents { get; set; } = new Dictionary<int, byte>();
    }
}
