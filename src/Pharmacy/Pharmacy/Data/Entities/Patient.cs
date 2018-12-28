using System.Collections.Generic;

namespace Pharmacy.Data.Entities
{
    public class Patient : Person
    {
        public Patient()
        {
            Insurances = new HashSet<PatientInsurance>();
        }

        public AppUser User { get; set; }
        public int UserId { get; set; }

        public virtual ICollection<PatientInsurance> Insurances { get; set; }
        public virtual ICollection<Script> Scripts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}