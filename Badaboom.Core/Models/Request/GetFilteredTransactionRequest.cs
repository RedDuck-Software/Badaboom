namespace Badaboom.Core.Models.Request
{
    public class GetFilteredTransactionRequest
    {
        public long? BlockNumber;

        public long? BlockNumberFrom;

        public long? BlockNumberTo;

        
        public string ContractAddress;

        public string MethodId;

        /// <summary>
        /// Pagination. Count of transaction to get from request
        /// </summary>
        public int Count; 
    }
}
