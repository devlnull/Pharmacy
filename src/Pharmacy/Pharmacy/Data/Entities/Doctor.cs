using System.Collections.Generic;

namespace Pharmacy.Data.Entities
{
    public class Doctor : Person
    {
        public Doctor()
        {
            Licenses = new HashSet<DoctorLicense>();
        }

        public AppUser User { get; set; }
        public int UserId { get; set; }

        public UserStates State { get; set; }

        public virtual ICollection<DoctorLicense> Licenses { get; set; }
        public virtual ICollection<Script> Scripts { get; set; }
    }
}
