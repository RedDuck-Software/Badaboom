using Badaboom.Core.Models.Enums;

namespace Badaboom.Core.Models.Request
{
    public class PurchaseRequest
    {
        public string TxnHash { get; set; }

        public ProductType ProductType { get; set; }

        public int Quantity { get; set; }
    }
}
