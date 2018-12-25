using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.MedicineCategory;
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
    public class MedicineCategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        public MedicineCategoryController(AppUserManager userManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _context.MedicineCategories
                .Include(x => x.Medicines)
                .OrderBy(x => x.Name)
                .ToListAsync();
            List<MedicineCategoryModel> model = new List<MedicineCategoryModel>();
            foreach (var item in categories)
            {
                int medicineCount = item.Medicines != null ? item.Medicines.Count : 0;
                model.Add(new MedicineCategoryModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Medicines = medicineCount
                });
            }
            return View(model);
        }

        public ActionResult Add()
        {
            MedicineCategoryModel model = new MedicineCategoryModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(MedicineCategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MedicineCategory medCat = new MedicineCategory();
                    medCat.Name = model.Name;
                    await _context.MedicineCategories.AddAsync(medCat);
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
            MedicineCategoryModel model = new MedicineCategoryModel();
            var medCat = await _context.MedicineCategories.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            model.Name = medCat.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(MedicineCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    TempData["message"] = ModelState.ErrorGathering();
                var medCat = await _context.MedicineCategories.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
                if (medCat != null)
                {
                    medCat.Name = model.Name;
                    _context.MedicineCategories.Update(medCat);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["message"] = "Medicine Category not found.";
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
            var item = await _context.MedicineCategories.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            if (item != null)
            {
                _context.MedicineCategories.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
