using Microsoft.AspNetCore.Identity;
using Pharmacy.Data.Entities;

namespace Pharmacy.Data
{
    public class AppUser : IdentityUser<int>
    {
        public virtual Employee Employee { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
