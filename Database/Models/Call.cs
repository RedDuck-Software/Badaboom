

namespace Database.Models
{
    public class Call
    {
		public int CallId { get; set; }

        public int TransactionId { get; set; }

		public string ContractAddress { get; set; }

        public string MethodId { get; set;  } 

        public Transaction Transaction { get; set; }
    }
}
