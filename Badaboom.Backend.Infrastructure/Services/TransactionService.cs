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

namespace Badaboom.Backend.Infrastructure.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Core.Models.Response.Transaction>> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request);
        Task<IEnumerable<Core.Models.Response.Transaction>> GetPaginatedFilteredTransactionsWithInputParameters(GetFilteredTransactionRequest request, int maxSearchTries);
        List<Nethereum.ABI.FunctionEncoding.ParameterOutput> DecodeInputData(DecodeInputDataRequest request, string inputData);
    }

    public class TransactionService : ITransactionService
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionStringIndexes;
        private readonly string _connectionString;
        private readonly Web3 _web3;


        public TransactionService(IConfiguration configuration, Web3 web3)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionStringIndexes = configuration.GetConnectionString("ETHIndexes");
            _web3 = web3;
        }


        public async Task<IEnumerable<Core.Models.Response.Transaction>> GetPaginatedFilteredTransactionsWithInputParameters(GetFilteredTransactionRequest request, int maxSearchTries)
        {
            if (request.DecodeInputDataInfo == null || request.ContractAddress == null)
                throw new System.ArgumentException("Contract address cannot be empty if DecodeInputDataInfo specified");

            List<Core.Models.Response.Transaction> res = new();

            int tries = 0;

            if (request.Count < 10)
                request.Count = 10;

            do
            {
                var txs = await this.GetPaginatedFilteredTransactions(request);

                if (txs == null || !txs.Any()) break;

                foreach (var tx in txs)
                {
                    try
                    {
                        if (ValidateInpuParameter(tx, request.DecodeInputDataInfo))
                            res.Add(tx);
                    }
                    catch (System.Exception ex) { }
                }

                request.Page += 1;
                tries++;
            } while (tries <= maxSearchTries && res.Count < request.Count);

            return res;
        }



        public async Task<IEnumerable<Core.Models.Response.Transaction>> GetPaginatedFilteredTransactions(GetFilteredTransactionRequest request)
        {


            IEnumerable<Call> res;

            using (var tRepo = new TransactionRepository(_connectionStringIndexes))
            {
                res = await tRepo.GetCalls(new CallsPagination()
                {
                    BlockId = request.BlockNumber,
                    MethodId = request.MethodId,
                    To = request.ContractAddress,
                    Page = request.Page,
                    Count = request.Count
                });
            }


            return res.Select(c =>
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
                    };
                }
            );
        }

        public List<Nethereum.ABI.FunctionEncoding.ParameterOutput> DecodeInputData(DecodeInputDataRequest request, string inputData)
        {
            var contract = _web3.Eth.GetContract(request.FunctionAbi, request.ContractAddress);

            var func = contract.GetFunction(request.MethodName);

            return func.DecodeInput(inputData);
        }

        private bool ValidateInpuParameter(Core.Models.Response.Transaction tx, DecodeInputDataRequest inputData)
        {
            var decodedInputFields = this.DecodeInputData(inputData, tx.Input);

            if (decodedInputFields is null) return false;

            foreach (var field in decodedInputFields)
            {
                if (field.Parameter.Name == inputData.FieldName && field.Result.ToString() == inputData.Value)
                    return true;
            }

            return false;
        }
    }
}
