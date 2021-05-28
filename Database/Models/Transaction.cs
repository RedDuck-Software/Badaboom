using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class Transaction
    {
		public int Id { get; set; }

		public string Hash { get; set; }

		public DateTime Time { get; set; }

		public IEnumerable<Call> Calls { get; set; }
	}
}
