using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Insurances;
using Pharmacy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Ability]
    [Authorize(Roles = PharmacyRoles.Employee)]
    public class InsuranceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        public InsuranceController(AppUserManager userManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<ActionResult> Index()
        {
            var insurances = await _context.Insurances
                .Include(x => x.Supports)
                .OrderBy(x => x.Name)
                .ToListAsync();
            List<InsuranceModel> model = new List<InsuranceModel>();
            foreach (var item in insurances)
            {
                int supportedMedicine = item.Supports != null ? item.Supports.Count : 0;
                model.Add(new InsuranceModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    SupportedMedicine = supportedMedicine
                });
            }
            return View(model);
        }

        public ActionResult Add()
        {
            InsuranceModel model = new InsuranceModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(InsuranceModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Insurance insur = new Insurance();
                    insur.Name = model.Name;
                    await _context.Insurances.AddAsync(insur);
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
            InsuranceModel model = new InsuranceModel();
            var medCat = await _context.Insurances.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            model.Name = medCat.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(InsuranceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    TempData["message"] = ModelState.ErrorGathering();
                var insur = await _context.Insurances.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
                if (insur != null)
                {
                    insur.Name = model.Name;
                    _context.Insurances.Update(insur);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["message"] = "Insurance not found.";
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
            var item = await _context.Insurances.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            if (item != null)
            {
                _context.Insurances.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
