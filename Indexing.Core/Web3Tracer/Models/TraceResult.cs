namespace Web3Tracer.Models
{
    public class TraceResult
    {
        public string CallType { get; set; }

        public string Error { get; set; }

        public string Input { get; set; }
        
        public string From { get; set; }

        public string To { get; set; }

        public string GasUsed { get; set; }

        public string Gas { get; set; }

        public string Value { get; set; }
        
        public string Output { get; set; }

        public string Time{ get; set; }

    }
}
