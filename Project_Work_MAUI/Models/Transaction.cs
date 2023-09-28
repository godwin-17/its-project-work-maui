namespace Project_Work_MAUI.Models
{
    public class Categoryid
    {
        public string category { get; set; }
        public string typology { get; set; }
        public string id { get; set; }
    }

    public class RootTransaction
    {
        public List<Transaction> transactions { get; set; }
    }
    public class Transaction
    {
        public DateTime date { get; set; }
        public int? balance { get; set; }
        public int amount { get; set; }
        public Categoryid categoryid { get; set; }
        public string description { get; set; }
    }
}
