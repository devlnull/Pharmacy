using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Scripts;
using Pharmacy.Services;
using Pharmacy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Authorize(Roles = PharmacyRoles.Patient)]
    public class PatientScriptController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IUserService _userService;

        public PatientScriptController(AppUserManager userManager, AppDbContext context,
            IUserService userService)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
            _userService = userService ?? throw new Exception(nameof(userService));
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient");
                var scripts = await _context.Scripts
                    .Where(x => x.PatientId.Equals(patient.Id))
                    .ToListAsync();
                return View(scripts);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Detail(int id)
        {
            try
            {
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient");
                var script = await _context.Scripts
                    .Include(x => x.Patient)
                    .Include(x => x.Doctor)
                    .Include(x => x.ScriptDetails).ThenInclude(y => y.Medicine)
                    .FirstOrDefaultAsync(x => x.PatientId.Equals(patient.Id) && x.Id.Equals(id));
                var order = await _context.Orders
                    .FirstOrDefaultAsync(x => x.ScriptId.Equals(script.Id));
                if (order != null)
                    if (order.Status == OrderStatuses.Ordered)
                        ViewData["AlreadyOrdered"] = true;
                return View(script);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Delete(int Id)
        {
            var item = await _context.Scripts.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            if (item != null)
            {
                _context.Scripts.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
