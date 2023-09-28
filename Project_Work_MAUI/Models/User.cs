namespace Project_Work_MAUI.Models
{
    public class RootUser
    {
        public User user { get; set; }
        public string token { get; set; }
    }
    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string picture { get; set; }
        public DateTime date { get; set; }
        public string iban { get; set; }
        public string fullName { get; set; }
        public string id { get; set; }

        public string FormattedDate
        {
            get
            {
                return date.ToString("dd MMMM yyyy");
            }
        }
    }
}
