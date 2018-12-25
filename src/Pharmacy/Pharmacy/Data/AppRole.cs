using Microsoft.AspNetCore.Identity;

namespace Pharmacy.Data
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole()
        {

        }

        public AppRole(string roleName) : base(roleName)
        {
        }
    }
}
