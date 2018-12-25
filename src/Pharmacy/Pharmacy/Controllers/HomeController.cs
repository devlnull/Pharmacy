using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.Models.Users;

namespace Pharmacy.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppUserManager _userManager;
        private readonly AppDbContext _context;
        public HomeController(AppUserManager userManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }
        public async Task<ActionResult> Index()
        {
            HomeModel model = new HomeModel();
            model.MedicineCategoryCount = _context.MedicineCategories.Count();
            model.MedicineCount = _context.Medicines.Count();
            model.OrderCount = _context.Orders.Count();
            model.ProductCount = _context.Products.Count();
            model.ScriptCount = _context.Scripts.Count();
            model.CompanyCount = _context.Companies.Count();
            model.DoctorLicenses = _context.DoctorLicenses.Count();
            model.InsuranceCount = _context.Insurances.Count();
            if (User.IsInRole(PharmacyRoles.Admin))
            {
                model.AdminCount = (await _userManager.GetUsersInRoleAsync(PharmacyRoles.Admin)).Count;
                model.EmployeeCount = (await _userManager.GetUsersInRoleAsync(PharmacyRoles.Employee)).Count;
                model.DoctorCount = (await _userManager.GetUsersInRoleAsync(PharmacyRoles.Doctor)).Count;
                model.PatientCount = (await _userManager.GetUsersInRoleAsync(PharmacyRoles.Patient)).Count;
            }
            else if (User.IsInRole(PharmacyRoles.Doctor))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var doc = await _context.Doctors
                    .Include(x => x.Licenses)
                    .Include(x => x.Scripts)
                    .FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                model.CurrentDoctorLicenses = doc.Licenses.Count;
                model.CurrentDoctorScripts = doc.Scripts.Count;
                model.Active = doc.State == UserStates.Active;
            }
            else if (User.IsInRole(PharmacyRoles.Employee))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                model.Active = emp.State == UserStates.Active;
            }
            else if (User.IsInRole(PharmacyRoles.Patient))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var patient = await _context.Patients.Include(x => x.Scripts).Include(x => x.Orders).FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                model.CurrentPatientScripts = patient.Scripts.Count();
                model.CurrentPatientOrders = patient.Orders.Count();
                model.CurrentPatientBasketItems = patient.Orders.Where(x => x.Status == OrderStatuses.Preorder).Count();
            }


            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
