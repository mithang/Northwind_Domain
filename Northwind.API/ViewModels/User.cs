namespace Northwind.API.Models
{
    public class AppSettings
    {
        public static AppSettings appSettings { get; set; }
        public string JwtSecret { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public string JwtEmailEncryption { get; set; }
    }
    public class User
    {
        public System.Guid id {get; set;}
        public string name {get; set;}
        public string email {get; set;}
        public string oauthSubject {get; set;}
        public string oauthIssuer {get; set;}
    }

    public class UserView
    {
        public string tokenId {get; set;}
    }
}