using Badaboom.Core.Models.Enums;
using Badaboom.Core.Models.Response;
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

        Task<ProductPriceResponse> GetProductPrice(ProductType productType);

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
            => _configuration.GetSection("NetworkSettings")["WaletAddress"];

        public async Task<ProductPriceResponse> GetProductPrice(ProductType productType)
        {
            using var pRepo = new PeymentRepository(_connectionString);

            var res = await pRepo.GetProductPrice(productType.ToString());

            return new ProductPriceResponse()
            {
                PricePerItem = res.Item1,
                AmountFromPercents = res.Item2
            };
        }

        public async Task<bool> ValidatePurchase(string txhash, string from, BigInteger amountToSend)
        {
            Nethereum.Web3.Web3 web3 = new(_configuration.GetSection("RPCUrls")["EthRopsten"]);
            var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txhash);

            bool _value = amountToSend.Equals(transaction.Value.Value);
            bool _from = from.ToLower() == transaction.From.ToLower();
            bool _to = GetWalletAddress().ToLower() == transaction.To.ToLower();

            return (_value && _from && _to);
        }

        public async Task<BigInteger> PurchaseCost(ProductType productType, int quantity)
        {
            var productPrice = await GetProductPrice(productType);

            var sorted = productPrice.AmountFromPercents.OrderByDescending(x => x.Key);

            foreach (var item in sorted)
            {
                if (quantity >= item.Key)
                {
                    return (BigInteger)productPrice.PricePerItem * quantity * item.Value / 100;
                }
            }

            return (BigInteger)productPrice.PricePerItem * quantity;
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
