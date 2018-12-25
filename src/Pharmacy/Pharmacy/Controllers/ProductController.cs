using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Services.ProductService;
using System;
using System.Linq;
using System.Threading.Tasks;
using Pharmacy.Models.Products;
using Pharmacy.Utilities;
using Microsoft.AspNetCore.Authorization;
using Pharmacy.Models;

namespace Pharmacy.Controllers
{
    [Ability]
    [Authorize(Roles = PharmacyRoles.Employee)]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IProductService _productService;
        public ProductController(AppUserManager userManager, AppDbContext context, IProductService productService)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
            _productService = productService ?? throw new Exception(nameof(productService));
        }


        public async Task<ActionResult> AddCompanyProduct(int Id)
        {
            var company = await _context.Companies.FindAsync(Id);
            if (company == null)
                ViewBag.CompanyName = "NULL";
            else
                ViewBag.CompanyName = company.Name;
            CompanyProductModel model = new CompanyProductModel();
            model.Company = company.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddCompanyProduct(CompanyProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _productService.AddCompanyProductAsync(model);
                }
                else
                    TempData["message"] = ModelState.ErrorGathering();
                return RedirectToAction("Index", "Company");
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Company");
            }
        }

        [AllowAnonymous]
        public async Task<JsonResult> MedicinesList(string medicine)
        {
            if (!string.IsNullOrWhiteSpace(medicine))
            {
                var medicines = await _context.Medicines
                    .Where(x => x.Name.Contains(medicine))
                    .OrderBy(x => x.Name)
                    .Select(x => new { Name = $"{x.Name} - {x.Dosage}", x.Id })
                    .ToListAsync();
                if (medicines.Any())
                    return Json(medicines);
                else
                    return Json(new object[0]);
            }
            return Json(new object[0]);
        }

        public async Task<ActionResult> Index(SearchModel model)
        {
            var products = _context.Products
                .Include(x => x.Company)
                .Include(x => x.Medicine)
                .Select(x => new ProductModel()
                {
                    Id = x.Id,
                    Company = x.Company.Name,
                    Medicine = x.Medicine.Name + " - " + x.Medicine.Dosage,
                    CreateDate = x.CreateDate,
                    ExpireDate = x.ExpireDate,
                    Price = x.Price,
                    Quantity = x.Quantity
                });
            if (model.OnlyExistence)
                products = products.Where(x => x.Quantity > 0);
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                products = products.Where(x => x.Company.Contains(model.Query) || x.Medicine.Contains(model.Query));
            }
            var result = await products.ToListAsync();
            return View(result);
        }
    }
}
