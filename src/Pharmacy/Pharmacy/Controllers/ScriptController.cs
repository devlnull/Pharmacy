using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Scripts;
using Pharmacy.Services;
using Pharmacy.Services.ScriptService;
using Pharmacy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Ability]
    public class ScriptController : Controller
    {
        private readonly IScriptService _scriptService;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        public ScriptController(AppDbContext context, AppUserManager userManager, IScriptService scriptService, IUserService userService)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
            _scriptService = scriptService ?? throw new ArgumentException(nameof(scriptService));
            _userService = userService ?? throw new ArgumentException(nameof(userService));
        }

        [Authorize(Roles = PharmacyRoles.Doctor)]
        public async Task<ActionResult> Index()
        {
            try
            {
                var doc = await _userService.GetCurrentUserAsync<Doctor>(UserTypes.Doctor) as Doctor;
                if (doc == null)
                    throw new Exception("user is not a doctor.");
                var scripts = await _context.Scripts
                    .Include(x => x.Patient)
                    .Include(x => x.Doctor)
                    .Where(x => x.Status == ScriptStatuses.Pending && x.DoctorId == doc.Id)
                    .ToListAsync();
                return View(scripts);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = PharmacyRoles.Patient)]
        public ActionResult RequestScript()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = PharmacyRoles.Patient)]
        public async Task<ActionResult> RequestScript(RequestScriptModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception(ModelState.ErrorGathering());
                await _scriptService.RequestNewScript(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = PharmacyRoles.Doctor)]
        public async Task<ActionResult> ResponseToScript(int id)
        {
            try
            {
                var item = await _context.Scripts.Include(x => x.Patient).FirstOrDefaultAsync(x => x.Id.Equals(id));
                if (item == null)
                    throw new Exception("script not found");
                var result = new RespondToScriptModel();
                result.Request = item.Request;
                result.PatientName = $"{item.Patient.Firstname} {item.Patient.Lastname}";
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = PharmacyRoles.Doctor)]
        public async Task<ActionResult> ResponseToScript(RespondToScriptModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception(ModelState.ErrorGathering());
                await _scriptService.RespondToScript(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
