using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models;
using Pharmacy.Models.Doctors;
using Pharmacy.Services;
using Pharmacy.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Ability]
    [Authorize(Roles = PharmacyRoles.Admin + "," + PharmacyRoles.Employee)]
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IUserService _userService;
        public DoctorController(AppUserManager userManager, AppDbContext context, IUserService userService)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
            _userService = userService ?? throw new Exception(nameof(userService));
        }

        public async Task<ActionResult> ListOfLicenses(SearchModel model)
        {
            var licenses = _context.DoctorLicenses.Where(x => true);
            licenses = licenses.Where(x => x.Status == model.DoctorLicenseStatus);
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                licenses = licenses.Where(x => x.Title.Contains(model.Query));
            }
            var result = await licenses
                .Include(x => x.Doctor)
                .Select(x => new LicenseModel() { Id = x.Id, DoctorName = x.Doctor.Firstname + " " + x.Doctor.Lastname, Title = x.Title, Status = x.Status })
                .ToListAsync();
            return View(result);
        }

        public async Task<ActionResult> AcceptLicense(int id)
        {
            try
            {
                var license = await _context.DoctorLicenses.FindAsync(id);
                if (license == null)
                    throw new Exception("License is not found!");
                license.Status = DoctorLicenseStatuses.Accepted;
                _context.DoctorLicenses.Update(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListOfLicenses));
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> DoctorList(string doctor)
        {
            if (!string.IsNullOrWhiteSpace(doctor))
            {
                var doctors = await _context.Doctors
                    .Where(x => x.Firstname.Contains(doctor) || x.Lastname.Contains(doctor))
                    .OrderBy(x => x.Lastname)
                    .Select(x => new { Name = x.Firstname + " " + x.Lastname, x.Id })
                    .ToListAsync();
                if (doctors.Any())
                    return Json(doctors);
                else
                    return Json(new object[0]);
            }
            return Json(new object[0]);
        }

        public async Task<ActionResult> RejectLicense(int id)
        {
            try
            {
                var license = await _context.DoctorLicenses.FindAsync(id);
                if (license == null)
                    throw new Exception("License is not found!");
                license.Status = DoctorLicenseStatuses.Rejected;
                _context.DoctorLicenses.Update(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListOfLicenses));
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
