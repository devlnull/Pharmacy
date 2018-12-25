namespace Pharmacy.Models.Admin
{
    public class AttemptedUser
    {
        public AttemptedUser(string username, string type)
        {
            Username = username;
            Type = type;
        }
        public string Username { get; set; }
        public string Type { get; set; }
    }
}
