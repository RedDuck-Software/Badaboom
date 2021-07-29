using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Database.Models;
using Database.Respositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Model;
using Nethereum.Web3;
using Newtonsoft.Json;
using System.Net.Http;

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface ITransactionService
    {
        Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request, bool isCountCalculatedNeded = true);
        Task<PaginationTransactionResponse> GetPaginatedFilteredTransactionsWithInputParameters(GetFilteredTransactionRequest request, int maxSearchTries);
        List<Nethereum.ABI.FunctionEncoding.ParameterOutput> DecodeInputData(DecodeInputDataRequest request, string contractAddress, string methodName, string inputData);
        Task<string> GetContractAbi(string contractAddress);

        Task<IEnumerable<InternalTxnResponse>> GetInternalTxns(string txhash);
    }

    class EtherscanResponse
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public string Result { get; set; }
    }

    public class TransactionService : ITransactionService
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionStringIndexes;
        private readonly string _connectionString;
        private readonly Web3 _web3;

        private readonly string EtherscanGetContractAbiUrl = "https://api.etherscan.io/api?module=contract&action=getabi&address={0}&apikey={1}";
        private readonly string EtherscanApiKey;

        private HttpClient HttpClient { get; set; }


        public TransactionService(IConfiguration configuration, Web3 web3, HttpClient client)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionStringIndexes = configuration.GetConnectionString("ETHIndexes");
            EtherscanApiKey = configuration.GetSection("NetworkSettings").GetSection("ETH").GetSection("EtherscanApiKey").Value;
            _web3 = web3;
            HttpClient = client;
        }

        public async Task<string> GetContractAbi(string contractAddress)
        {
            var res = await HttpClient.GetAsync(string.Format(EtherscanGetContractAbiUrl, contractAddress, EtherscanApiKey));

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var content = await res.Content.ReadAsStringAsync();

            var deserializedContent = JsonConvert.DeserializeObject<EtherscanResponse>(content);

            return deserializedContent.Status == "1" ? deserializedContent.Result : null;
        }


        public async Task<PaginationTransactionResponse> GetPaginatedFilteredTransactionsWithInputParameters(GetFilteredTransactionRequest request, int maxSearchTries)
        {
            if (request.DecodeInputDataInfo == null || request.ContractAddress == null)
                throw new System.ArgumentException("Contract address cannot be empty if DecodeInputDataInfo specified");

            List<Core.Models.Response.Transaction> res = new();

            int tries = 0;

            if (request.Count < 10)
                request.Count = 10;

            do
            {
                var txs = await this.GetPaginatedFilteredTransactions(request, false);

                if (txs == null || !txs.Transactions.Any()) break;

                foreach (var tx in txs.Transactions)
                {
                    try
                    {
                        if (ValidateInpuParameter(tx, request))
                            res.Add(tx);
                    }
                    catch (System.Exception ex) { }
                }

                request.Page += 1;
                tries++;
            } while (tries <= maxSearchTries && res.Count < request.Count);

            return new PaginationTransactionResponse() 
            {
                Count = 0,
                Transactions = res
            };
        }



        public async Task<PaginationTransactionResponse> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request, bool isCountCalculatedNeded = true)
        {
            (IEnumerable<Call> transactions, int? totalCount) res;

            using (var tRepo = new TransactionRepository(_connectionStringIndexes))
            {
                res = await tRepo.GetCalls(new CallsPagination()
                {
                    BlockId = request.BlockNumber,
                    MethodId = request.MethodId,
                    From = request.From,
                    To = request.ContractAddress,
                    Page = request.Page,
                    Count = request.Count,
                    CallIdFrom = request.CallId
                }, isCountCalculatedNeded);
            }

            return new PaginationTransactionResponse()
            {
                Count = res.totalCount ?? 0,
                Transactions = res.transactions.Select(c =>
                {
                    return new Core.Models.Response.Transaction()
                    {
                        Method = c.MethodId,
                        Block = (ulong)c.BlockId,
                        From = c.From,
                        To = c.To,
                        TxnHash = c.TransactionHash,
                        Age = (ulong)c.TimeStamp,
                        Input = c.Input,
                        CallId = c.CallId
                    };
                })
            };
        }

        public List<Nethereum.ABI.FunctionEncoding.ParameterOutput> DecodeInputData(DecodeInputDataRequest request, string contractAddress, string methodName, string inputData)
        {
            string serializedAbi = JsonConvert.SerializeObject(request.FunctionAbis);
            var contract = _web3.Eth.GetContract(serializedAbi, contractAddress);

            var func = contract.GetFunction(methodName);

            return func.DecodeInput(inputData);
        }

        private bool ValidateInpuParameter(Core.Models.Response.Transaction tx, GetFilteredTransactionRequest request)
        {
            var decodedInputFields = this.DecodeInputData(request.DecodeInputDataInfo, request.ContractAddress, request.DecodeInputDataInfo.FunctionAbis?.First()?.Name, tx.Input);

            if (decodedInputFields is null) return false;

            for (int i = 0; i < request.DecodeInputDataInfo.ArgumentsNamesValues.Count; i++)
                foreach (var field in decodedInputFields)
                    if (request.DecodeInputDataInfo.ArgumentsNamesValues.ContainsKey(field.Parameter.Name))
                        if (field.Result.ToString().ToLower() != request.DecodeInputDataInfo.ArgumentsNamesValues[field.Parameter.Name].ToLower())
                            return false;

            return true;
        }

        public async Task<IEnumerable<InternalTxnResponse>> GetInternalTxns(string txhash)
        {
            IEnumerable<Call> res;
            using (var tRepo = new TransactionRepository(_connectionStringIndexes))
            {
                res = await tRepo.GetInternalTransactions(txhash);
            };

            return res.Select(c => 
            {
                return new InternalTxnResponse()
                {
                    Type = (Core.Models.Enums.CallTypes)c.Type,
                    From = c.From,
                    To = c.To
                };
            });
        }
    }
}
