using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models.Employee;
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
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        public EmployeeController(AppUserManager userManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<ActionResult> OrderList()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(x => x.Patient)
                    .Include(x => x.OrderDetails)
                    .Where(x => x.Status == OrderStatuses.Ordered).ToListAsync();
                List<OrderInfoModel> model = new List<OrderInfoModel>();
                var details = await _context.OrderDetails.Include(x => x.Product).ToListAsync();

                foreach (var item in orders)
                {
                    var totalPrice = details.Where(x => x.OrderId.Equals(item.Id)).Sum(x => x.Product.Price);
                    model.Add(new OrderInfoModel()
                    {
                        ItemCount = item.OrderDetails.Count(),
                        PatientName = item.Patient.Firstname + " " + item.Patient.Lastname,
                        TotalPrice = totalPrice,
                    });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> ScriptList()
        {
            try
            {
                var scripts = await _context.Scripts
                    .Include(x => x.Patient)
                    .Include(x => x.Order)
                    .Include(x => x.Doctor)
                    .Include(x => x.ScriptDetails)
                    .ToListAsync();
                List<ScriptInfoModel> model = new List<ScriptInfoModel>();
                var details = await _context.OrderDetails.Include(x => x.Product).ToListAsync();

                foreach (var item in scripts)
                {
                    model.Add(new ScriptInfoModel()
                    {
                        DoctorName = item.Doctor.Firstname + " " + item.Doctor.Lastname,
                        PatientName = item.Patient.Firstname + " " + item.Patient.Lastname,
                        OrderStatus = item.Order == null ? "NOT ORDERED" : item.Order.Status.ToString(),
                        ScriptStatus = item.Status,
                        MedicineScripted = item.ScriptDetails.Count()
                    });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
