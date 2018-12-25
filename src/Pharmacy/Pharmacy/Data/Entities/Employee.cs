namespace Pharmacy.Data.Entities
{
    public class Employee : Person
    {
        public AppUser User { get; set; }
        public int UserId { get; set; }
        public UserStates State { get; set; }
    }
}
