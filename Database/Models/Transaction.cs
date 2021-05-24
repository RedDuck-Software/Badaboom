using System;

namespace Database.Models
{
    public class Transaction
    {
		public int Id { get; set; }

		public string ContractAddress { get; set; }

		public string Hash { get; set; }

		public string MethodId { get; set; }

		public DateTime Time { get; set; }
	}
}
