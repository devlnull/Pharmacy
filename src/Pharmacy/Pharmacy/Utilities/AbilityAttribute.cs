using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pharmacy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pharmacy.Utilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AbilityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string errorMessage = "You are not able to do this action.";
                AppDbContext _context = (AppDbContext)context.HttpContext.RequestServices.GetService(typeof(AppDbContext)) as AppDbContext;
                AppUserManager _userManager = context.HttpContext.RequestServices.GetService(typeof(AppUserManager)) as AppUserManager;
                if (_context == null || _userManager == null)
                    return;
                var user = _userManager.FindByNameAsync(context.HttpContext.User.Identity.Name).Result;
                if (context.HttpContext.User.IsInRole(PharmacyRoles.Employee))
                {
                    var emp = _context.Employees.FirstOrDefault(x => x.UserId.Equals(user.Id));
                    if (emp == null)
                        context.Result = new BadRequestObjectResult(errorMessage);
                    if (emp.State == UserStates.Deactive)
                    {
                        context.Result = new BadRequestObjectResult(errorMessage);
                    }
                }
                else if (context.HttpContext.User.IsInRole(PharmacyRoles.Doctor))
                {
                    var doc = _context.Doctors.FirstOrDefault(x => x.UserId.Equals(user.Id));
                    if (doc == null)
                        context.Result = new BadRequestObjectResult(errorMessage);
                    if (doc.State == UserStates.Deactive)
                    {
                        context.Result = new BadRequestObjectResult(errorMessage);
                    }
                }
            }
        }
    }
}
