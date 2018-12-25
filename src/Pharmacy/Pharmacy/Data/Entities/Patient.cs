using System.Collections.Generic;

namespace Pharmacy.Data.Entities
{
    public class Patient : Person
    {
        public Patient()
        {
            Insurances = new HashSet<Insurance>();
        }

        public AppUser User { get; set; }
        public int UserId { get; set; }

        public virtual ICollection<Insurance> Insurances { get; set; }
        public virtual ICollection<Script> Scripts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}