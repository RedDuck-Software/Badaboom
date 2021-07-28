using Badaboom.Core.Models.Enums;
using Database.Respositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface IPaymentService
    {
        // method: add purchase to a transaction history (in future +table)

        string GetWalletAddress();

        Task<long> GetPricePerItem(ProductType productType);

        Task<BigInteger> PurchaseCost(ProductType productType, int quantity);

        Task<bool> ValidatePurchase(string txhash, string from, BigInteger amountToSend); // use only after making payment
        
        Task SetProduct(string address, ProductType productType, int quantity);

        Task<int?> CheckQuantity(ProductType productType, string address);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly HttpClient _httpClient;


        public PaymentService(
            IConfiguration configuration,
            HttpClient httpClient
            )
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpClient = httpClient;
        }

        public string GetWalletAddress()
            => _configuration.GetSection("NetworkSettings").GetSection("ETH")["WaletAddress"];

        public async Task<long> GetPricePerItem(ProductType productType)
        {
            using var pRepo = new PeymentRepository(_connectionString);

            long pricePerItem = await pRepo.GetProductPrice(productType.ToString());

            return pricePerItem;
        }

        public async Task<bool> ValidatePurchase(string txhash, string from, BigInteger amountToSend)
        {
            Nethereum.Web3.Web3 web3 = new("https://ropsten.infura.io/v3/89b011b73e644d77a85bad2a0cbe4e61");
            var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txhash);

            bool _value = amountToSend.Equals(transaction.Value.Value);
            bool _from = from.ToLower() == transaction.From.ToLower();
            bool _to = GetWalletAddress().ToLower() == transaction.To.ToLower();

            return (_value && _from && _to);
        }

        public async Task<BigInteger> PurchaseCost(ProductType productType, int quantity)
        {
            long pricePerItem = await GetPricePerItem(productType);

            return pricePerItem * quantity;
        }

        public async Task SetProduct(string address, ProductType productType, int quantity)
        {
            using var pRepo = new PeymentRepository(_connectionString);

            int? currentQuantity = await pRepo.CheckQuantityArgumentFunctionRequests(address, productType.ToString());


            if (currentQuantity == null)
            {
                await pRepo.AddUserProduct(address, productType.ToString(), quantity);
            }
            else if (currentQuantity <= 1 && quantity == -1)
            {
                await pRepo.DeleteUserProduct(address, productType.ToString());
            }
            else
            {
                await pRepo.UpdateProductQuantity(address, productType.ToString(), (int)currentQuantity + quantity);
            }
        }

        public async Task<int?> CheckQuantity(ProductType productType, string address)
        {
            int? result = default;

            if (productType == ProductType.ArgumentFunctionRequests)
            {
                using var pRepo = new PeymentRepository(_connectionString);
                    
                result = await pRepo.CheckQuantityArgumentFunctionRequests(address, productType.ToString());
            }

            return result;
        }
    }
}
