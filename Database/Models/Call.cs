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

        public Transaction Transaction { get; set; }

        public CallTypes Type { get; set; }

        public static CallTypes CreataCallTypeFromString(string type)
        {
            switch (type.ToLower())
            {
                case "create" : return CallTypes.Create;
                case "call": return CallTypes.Create;
                case "delegatecall": return CallTypes.Create;
                case "staticcall": return CallTypes.Create;
                case "callcode": return CallTypes.Create;
                default: return CallTypes.NO_CALL_TYPE;
            }
        }
    }

    public enum CallTypes
    {
        NO_CALL_TYPE,
        Create, 
        Call, 
        Delegatecall, 
        StaticCall, 
        Callcode
    }
}
