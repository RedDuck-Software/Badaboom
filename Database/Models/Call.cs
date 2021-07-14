using System;

namespace Database.Models
{
    public class Call
    {
        public long CallId { get; set; }

        public string TransactionHash { get; set; }

        public string Error { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string MethodId { get; set; }
        
        public string Input { get; set; }

        public Transaction Transaction { get; set; }

        public CallTypes Type { get; set; }
    }


    public class CallsPagination
    {
        public long? BlockId;
        public string MethodId;
        public string To;
    }

    public enum CallTypes
    {
        NO_CALL_TYPE,
        Create, 
        Call, 
    }
}
