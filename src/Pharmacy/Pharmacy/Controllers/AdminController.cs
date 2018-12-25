using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models.Admin;
using Pharmacy.Models.Users;
using Pharmacy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Authorize(Roles = PharmacyRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;

        public AdminController(IUserService userService, AppUserManager userManager, AppDbContext context)
        {
            _userService = userService ?? throw new Exception(nameof(userService));
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }

        public ActionResult NewAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> NewAdmin(RegisterAdminModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.RegisterAdminAsync(model);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }
            }
            else
                return View();
        }

        public async Task<ActionResult> ListOfAttempts()
        {
            List<AttemptedUser> attemptedUsers = new List<AttemptedUser>();
            try
            {
                var docs = _context.Doctors.Include(x => x.User).Where(x => x.State == UserStates.Deactive);
                var doctorsAttempted = docs.Select(x => new AttemptedUser(x.User.UserName, UserTypes.Doctor.ToString()));
                var emps = _context.Employees.Include(x => x.User).Where(x => x.State == UserStates.Deactive);
                var employeeAttempted = emps.Select(x => new AttemptedUser(x.User.UserName, UserTypes.Employee.ToString()));
                attemptedUsers = await doctorsAttempted.Union(employeeAttempted).ToListAsync();
                return View(attemptedUsers);
            }
            catch(Exception ex)
            {
                TempData["message"] = ex.Message;
                return View(attemptedUsers);
            }
        }

        public async Task<ActionResult> ActivateUser(string username, string type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (type.ToLower().Equals(UserTypes.Doctor.ToString().ToLower()))
                    {
                        await _userService.ActiveDoctorAsync(username);
                    }
                    else if (type.ToLower().Equals(UserTypes.Employee.ToString().ToLower()))
                    {
                        await _userService.ActiveEmployeeAsync(username);
                    }
                    return RedirectToAction(nameof(ListOfAttempts));
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.Message;
                    return RedirectToAction(nameof(ListOfAttempts));
                }
            }
            else
                return RedirectToAction(nameof(ListOfAttempts));
        }

        public async Task<ActionResult> DeactivateUser(string username, string type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (type.ToLower().Equals(UserTypes.Doctor.ToString().ToLower()))
                    {
                        await _userService.DeactiveDoctorAsync(username);
                    }
                    else if (type.ToLower().Equals(UserTypes.Employee.ToString().ToLower()))
                    {
                        await _userService.DeactiveEmployeeAsync(username);
                    }
                    return RedirectToAction(nameof(ListOfAttempts));
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.Message;
                    return RedirectToAction(nameof(ListOfAttempts));
                }
            }
            else
                return RedirectToAction(nameof(ListOfAttempts));
        }
    }
}
