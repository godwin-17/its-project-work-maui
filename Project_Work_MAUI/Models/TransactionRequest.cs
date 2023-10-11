namespace Project_Work_MAUI.Models
{
    public class TransactionRequest
    {
        public string? iban { get; set; }
        public float amount { get; set; }
        public string categoryid { get; set; }
        public string description { get; set; }
    }
}
