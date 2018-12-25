using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Doctors;
using System.Threading.Tasks;
using Pharmacy.Services;
using Pharmacy.Utilities;
using System;

namespace Pharmacy.Controllers
{
    [Ability]
    [Authorize(Roles = PharmacyRoles.Doctor)]
    public class DoctorLicenseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IUserService _userService;
        public DoctorLicenseController(AppUserManager userManager, AppDbContext context, IUserService userService)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
            _userService = userService ?? throw new Exception(nameof(userService));
        }


        public async Task<ActionResult> Index()
        {
            try
            {
                var currentDoctor = await _userService.GetCurrentUserAsync<Doctor>(UserTypes.Doctor) as Doctor;
                if (currentDoctor == null)
                    throw new Exception("user is not a doctor");
                var doc = await _context.Doctors.Include(x => x.Licenses)
                    .FirstOrDefaultAsync(x => x.Id.Equals(currentDoctor.Id));
                if(doc == null)
                    throw new Exception("doctor not found");
                return View(doc.Licenses);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult NewLicense()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> NewLicense(LicenseModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var doc = await _userService.GetCurrentUserAsync<Doctor>(UserTypes.Doctor) as Doctor;
                    if (doc == null)
                        throw new Exception("user is not a doctor");
                    Data.Entities.DoctorLicense license = new Data.Entities.DoctorLicense();
                    license.Title = model.Title;
                    license.DoctorId = doc.Id;
                    license.Status = DoctorLicenseStatuses.Pending;
                    await _context.DoctorLicenses.AddAsync(license);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["message"] = ModelState.ErrorGathering();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> DeleteLicense(int id)
        {
            try
            {
                var license = await _context.DoctorLicenses.FindAsync(id);
                if (license == null)
                    throw new Exception("license not found");
                if (license.Status != DoctorLicenseStatuses.Pending)
                    throw new Exception("license status must be Pending. (otherwise you are not able to delete it)");
                _context.DoctorLicenses.Remove(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
