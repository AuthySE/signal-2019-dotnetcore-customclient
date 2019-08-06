namespace signal_2019_dotnetcore.Models
{
    public class AuthyUser
    {
        public string Username { get; set; }
        public string Id { get; set; }
        public string PasswordHash { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}