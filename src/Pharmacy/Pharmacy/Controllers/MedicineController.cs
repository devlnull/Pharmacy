using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models;
using Pharmacy.Models.Medicines;
using Pharmacy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Ability]
    [Authorize(Roles = PharmacyRoles.Employee)]
    [EnableCors(Startup.corsPolicyName)]
    public class MedicineController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        public MedicineController(AppUserManager userManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<ActionResult> Index(SearchModel model)
        {
            var medicines = _context.Medicines.Include(x => x.InsuranceSupports)
                .Include(x => x.Products)
                .Where(x => true);
            if (model.OnlyExistence)
                medicines = medicines.Where(x => x.InsuranceSupports.Count > 0);
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                medicines = medicines.Where(x => x.Name.Contains(model.Query) || x.Dosage.Contains(model.Query));
            }
            var result = await medicines.OrderBy(x => x.Name).ToListAsync();


            List<MedicineModel> resultModel = new List<MedicineModel>();
            foreach (var item in result)
            {
                int supportedByInsurances = item.InsuranceSupports != null ? item.InsuranceSupports.Count : 0;
                int producedByCompanies = item.Products != null ? item.Products.Count : 0;
                resultModel.Add(new MedicineModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    SupportedByInsurances = supportedByInsurances,
                    ProducedByCompanies = producedByCompanies,
                    Dosage = item.Dosage,
                });

            }
            return View(resultModel);
        }

        public async Task<List<SelectListItem>> GetCategories()
        {
            List<SelectListItem> Items = new List<SelectListItem>();
            var categories = await _context.MedicineCategories.ToListAsync();
            foreach (var item in categories)
                Items.Add(new SelectListItem(item.Name, item.Id.ToString()));
            return Items;
        }

        [AllowAnonymous]
        public async Task<ActionResult> FindMedicine(int id)
        {
            var item = await _context.Medicines.FindAsync(id);
            if (item == null)
                return Json(new object[0]);
            else
                return Json(item);
        }

        public async Task<ActionResult> Add()
        {
            MedicineModel model = new MedicineModel();
            model.Categories = await GetCategories();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(MedicineModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Medicine med = new Medicine();
                    med.CategoryId = model.Category;
                    med.Name = model.Name;
                    med.Dosage = model.Dosage;
                    await _context.Medicines.AddAsync(med);
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

        public async Task<ActionResult> Edit(int Id)
        {
            MedicineModel model = new MedicineModel();
            model.Categories = await GetCategories();
            var med = await _context.Medicines.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            model.Name = med.Name;
            model.Dosage = med.Dosage;
            model.Category = med.CategoryId;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(MedicineModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    TempData["message"] = ModelState.ErrorGathering();
                var med = await _context.Medicines.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
                if (med != null)
                {
                    med.Name = model.Name;
                    med.Dosage = model.Dosage;
                    _context.Medicines.Update(med);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["message"] = "Medicine not found.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> Delete(int Id)
        {
            var item = await _context.Medicines.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            if (item != null)
            {
                _context.Medicines.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
