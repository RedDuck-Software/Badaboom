using Badaboom.Core.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface IPaymentService
    {
        // method: add purchase to a transaction history (in future +table)

        string GetWalletAddress();

        decimal PurchaseCost(ProductType productType, int quantity);

        bool ValidatePurchase(string txhash, decimal amountToSend); // use only after making payment
        
        bool AddPurchase(ProductType productType, int quantity);
        
        int CheckQuantity(ProductType productType, string address);
        
        bool UseProduct(ProductType productType);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;


        public PaymentService(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string GetWalletAddress()
            => _configuration.GetSection("NetworkSettings").GetSection("ETH")["WaletAddress"];

        public bool ValidatePurchase(string txhash, decimal amountToSend)
        {
            throw new NotImplementedException();
        }

        public decimal PurchaseCost(ProductType productType, int quantity)
        {
            throw new NotImplementedException();
        }

        public bool AddPurchase(ProductType productType, int quantity)
        {
            throw new NotImplementedException();
        }

        public int CheckQuantity(ProductType productType, string address)
        {
            throw new NotImplementedException();
        }

        public bool UseProduct(ProductType productType)
        {
            throw new NotImplementedException();
        }
    }
}
