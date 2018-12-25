using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Companies;
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
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        public CompanyController(AppUserManager userManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<ActionResult> Index()
        {
            var comapnies = await _context.Companies.Include(x => x.Products)
                .OrderBy(x => x.Name)
                .ToListAsync();
            List<CompanyModel> model = new List<CompanyModel>();
            foreach (var item in comapnies)
            {
                int products = item.Products != null ? item.Products.Count : 0;
                model.Add(new CompanyModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Products = products
                });

            }
            return View(model);
        }

        public ActionResult Add()
        {
            CompanyModel model = new CompanyModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(CompanyModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Company com = new Company();
                    com.Name = model.Name;
                    await _context.Companies.AddAsync(com);
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
            CompanyModel model = new CompanyModel();
            var com = await _context.Companies.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            model.Name = com.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CompanyModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    TempData["message"] = ModelState.ErrorGathering();
                var com = await _context.Companies.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
                if (com != null)
                {
                    com.Name = model.Name;
                    _context.Companies.Update(com);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["message"] = "Company not found.";
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
            var item = await _context.Companies.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            if (item != null)
            {
                _context.Companies.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
