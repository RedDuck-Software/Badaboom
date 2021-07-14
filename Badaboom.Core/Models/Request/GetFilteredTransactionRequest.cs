namespace Badaboom.Core.Models.Request
{
    public class GetFilteredTransactionRequest
    {
        public long? BlockNumber { get; set; }

        public long? BlockNumberFrom { get; set; }

        public long? BlockNumberTo { get; set; }


        public string ContractAddress { get; set; }

        public string MethodId { get; set; }

        /// <summary>
        /// Pagination. Count of transaction to get from request
        /// </summary>
        public int Count { get; set; } = 10;

        public int Page { get; set; } = 1;

    }

}
