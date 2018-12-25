using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models.InsuranceSupport;
using Pharmacy.Models.Medicines;
using Pharmacy.Services.InsuranceService;
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
    public class InsuranceSupportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IInsuranceSupportService _insuranceSupportService;
        public InsuranceSupportController(AppUserManager userManager, AppDbContext context,
            IInsuranceSupportService insuranceSupportService)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
            _insuranceSupportService = insuranceSupportService ?? throw new Exception(nameof(insuranceSupportService));
        }


        public async Task<ActionResult> Index(int id)
        {
            try
            {
                var insurance = await _context.Insurances
                    .Include(x => x.Supports)
                    .FirstOrDefaultAsync(x => x.Id.Equals(id));
                if (insurance == null)
                    TempData["message"] = "Insurance not found";
                var supported = _context.InsuranceSupports
                    .Include(x => x.Medicine)
                    .Where(x => x.InsuranceId.Equals(insurance.Id))
                    .Select(x => x.Medicine)
                    .Include(x=>x.Products);
                List<MedicineModel> model = new List<MedicineModel>();
                foreach (var item in supported)
                {
                    int supportedByInsurances = item.InsuranceSupports != null ? item.InsuranceSupports.Count : 0;
                    int producedByCompanies = item.Products != null ? item.Products.Count : 0;
                    model.Add(new MedicineModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        SupportedByInsurances = supportedByInsurances,
                        ProducedByCompanies = producedByCompanies,
                        Dosage = item.Dosage,
                    });
                }
                ViewBag.Insurance = insurance.Name;
                ViewBag.InsuranceId = insurance.Id;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = TempData["message"] == null ? TempData["message"] + ex.Message : TempData["message"];
            }
            return RedirectToAction(nameof(Index),new { id});
        }

        public async Task<ActionResult> SupportMedicine(int id)
        {
            var insurance = await _context.Insurances
                .Include(x => x.Supports)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (insurance == null)
            {
                TempData["message"] = "Insurance not found";
                return RedirectToAction(nameof(Index), new { id });
            }
            InsuranceSupportModel model = new InsuranceSupportModel();
            model.Insurance = insurance.Id;
            model.InsuranceName = insurance.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SupportMedicine(InsuranceSupportModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    TempData["message"] = ModelState.ErrorGathering();
                await _insuranceSupportService.SupportMedicineAsync(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { id = model.Insurance });
        }

        public async Task<JsonResult> MedicinesList(int id, string medicine)
        {
            if (!string.IsNullOrWhiteSpace(medicine))
            {
                var insurance = await _context.Insurances.Include(x => x.Supports)
                    .FirstOrDefaultAsync(x => x.Id.Equals(id));
                if (insurance == null)
                    return Json(new object[0]);
                var supported = _context.InsuranceSupports
                    .Include(x => x.Medicine)
                    .Where(x => x.InsuranceId.Equals(insurance.Id))
                    .Select(x => x.Medicine);
                var all = (await _context.Medicines.Where(x => x.Name.Contains(medicine)).ToListAsync());
                if (supported.Any())
                    all = all.Except(supported).ToList();

                var medicines = all
                    .OrderBy(x => x.Name)
                    .Select(x => new { Name = $"{x.Name} - {x.Dosage}", x.Id });
                if (medicines.Any())
                    return Json(medicines);
                else
                    return Json(new object[0]);
            }
            return Json(new object[0]);
        }

    }
}
