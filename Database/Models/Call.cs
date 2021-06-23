namespace Database.Models
{
    public class Call
    {
        public int CallId { get; set; }

        public string TransactionHash { get; set; }


        public string Error { get; set; }


        public string From { get; set; }

        public string To { get; set; }


        public string MethodId { get; set; }

        public string Time { get; set; }


        public Transaction Transaction { get; set; }

        public string Type { get; set; }
    }
}
