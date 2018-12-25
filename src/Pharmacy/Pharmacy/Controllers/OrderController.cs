using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Orders;
using Pharmacy.Services;
using Pharmacy.Services.OrderService;
using Pharmacy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Ability]
    [Authorize(Roles = PharmacyRoles.Patient)]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public OrderController(AppUserManager userManager, AppDbContext context,
            IOrderService orderService, IUserService userService)
        {
            _userManager = userManager ?? throw new Exception(nameof(userManager));
            _context = context ?? throw new Exception(nameof(context));
            _orderService = orderService ?? throw new Exception(nameof(orderService));
            _userService = userService ?? throw new Exception(nameof(userService));
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient.");
                var orders = await _context.Orders.Include(x => x.OrderDetails)
                    .Where(x => x.PatientId.Equals(patient.Id)).ToListAsync();
                List<OrderInfoModel> orderInfoModel = new List<OrderInfoModel>();
                var orderIds = orders.Select(x => x.Id);
                var details = await _context.OrderDetails.Include(x => x.Product).Where(x => orderIds.Contains(x.OrderId)).ToListAsync();
                foreach (var item in orders)
                {
                    var totalPrice = details.Where(x=>x.OrderId.Equals(item.Id)).Sum(x => x.Product.Price);
                    var status = item.Status;
                    orderInfoModel.Add(new OrderInfoModel() { Status = status, TotalPrice = totalPrice });
                }
                return View(orderInfoModel);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> NewOrder(int id)
        {
            try
            {
                OrderListModel orderList = new OrderListModel();
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient!");
                var patientInsurances = await _context.PatientInsurances.Include(x => x.Insurance).ThenInclude(x => x.Supports)
                    .Where(x => x.PatientId.Equals(patient.Id)).ToListAsync();
                var script = await _context.Scripts.Include(x => x.ScriptDetails)
                    .FirstOrDefaultAsync(x => x.Id.Equals(id));
                if (script == null)
                    throw new Exception("script not found");
                var scriptDetailsProducts = script.ScriptDetails.Select(x => x.MedicineId).ToList();
                var availableProducts = _context.Products.Include(x => x.Medicine)
                    .Include(x => x.Company)
                    .Where(x => scriptDetailsProducts.Contains(x.MedicineId));
                List<OrderModel> products = new List<OrderModel>();
                var patientInsurancesIds = patientInsurances.Select(x => x.InsuranceId);
                var patientInsurancesSupports = await _context.InsuranceSupports
                    .Where(x => patientInsurancesIds.Contains(x.InsuranceId)).ToListAsync();
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.ScriptId.Equals(script.Id));
                List<OrderModel> basketItems = new List<OrderModel>();
                List<Product> availableBasketItems = new List<Product>();
                if (order != null)
                {
                    var orderDetails = await _context.OrderDetails
                        .Include(x => x.Product).ThenInclude(x => x.Medicine)
                        .Include(x => x.Product).ThenInclude(x => x.Company)
                        .Where(x => x.OrderId.Equals(order.Id)).ToListAsync();
                    var orderedDetails = orderDetails.Where(x => x.Status == OrderStatuses.Ordered);
                    var orderedDetailsIds = orderedDetails.Select(x => x.Product.MedicineId);
                    availableBasketItems = orderDetails.Select(x => x.Product).ToList();
                    var availableBasketItemsIds = availableBasketItems.Select(x => x.MedicineId);
                    //var orderedItems = orderDetails.Select(x=>x.Product).ToList(
                    availableProducts = availableProducts.Where(x => !availableBasketItemsIds.Contains(x.MedicineId));
                    availableProducts = availableProducts.Where(x => !orderedDetailsIds.Contains(x.MedicineId));

                    orderDetails = orderDetails.Where(x => !orderedDetailsIds.Contains(x.Product.MedicineId)).ToList();

                    foreach (var oDetails in orderDetails)
                    {
                        var supportedByInsurances = patientInsurancesSupports.Intersect(oDetails.Product.Medicine.InsuranceSupports);
                        bool supportedByInsurance = supportedByInsurances.Any();
                        var supportedByInsuranceIds = supportedByInsurances.Select(x => x.InsuranceId);
                        var supportedByInsuranceNames = (await _context.Insurances.Where(x => supportedByInsuranceIds.Contains(x.Id))
                            .ToListAsync())
                            .Select(x => x.Name);
                        basketItems.Add(new OrderModel()
                        {
                            Id = oDetails.Id,
                            ScriptId = script.Id,
                            CompanyName = oDetails.Product.Company.Name,
                            ProductExistence = oDetails.Product.Quantity,
                            ProductExpireDate = oDetails.Product.ExpireDate,
                            ProductPrice = supportedByInsurance == true ? 0 : oDetails.Product.Price,
                            MedicineName = oDetails.Product.Medicine.Name,
                            MedicineDosage = oDetails.Product.Medicine.Dosage,
                            SupportByInsurance = supportedByInsurance,
                            Insurance = string.Join(',', supportedByInsuranceNames)
                        });
                    }
                }
                orderList.Basket = basketItems;

                foreach (var item in availableProducts)
                {
                    var supportedByInsurances = patientInsurancesSupports.Intersect(item.Medicine.InsuranceSupports);
                    bool supportedByInsurance = supportedByInsurances.Any();
                    var supportedByInsuranceIds = supportedByInsurances.Select(x => x.InsuranceId);
                    var supportedByInsuranceNames = (await _context.Insurances.Where(x => supportedByInsuranceIds.Contains(x.Id))
                        .ToListAsync())
                        .Select(x => x.Name);
                    products.Add(new OrderModel()
                    {
                        Id = item.Id,
                        ScriptId = script.Id,
                        CompanyName = item.Company.Name,
                        MedicineName = item.Medicine.Name,
                        MedicineDosage = item.Medicine.Dosage,
                        ProductExpireDate = item.ExpireDate,
                        ProductPrice = item.Price,
                        ProductExistence = item.Quantity,
                        SupportByInsurance = supportedByInsurance,
                        Insurance = string.Join(',', supportedByInsuranceNames)
                    });
                }
                orderList.Products = products;

                return View(orderList);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Checkout()
        {
            try
            {
                await _orderService.CheckOutAsync();
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> AddToBasket(int productId, int scriptId)
        {
            try
            {
                var script = await _context.Scripts.FirstOrDefaultAsync(x => x.Id.Equals(scriptId));
                if (script == null)
                    throw new Exception("script not found.");
                var orderResult = await _orderService.OrderNewItemAsync(new NewOrderModel()
                {
                    Script = script.Id,
                    Products = new List<int>() { productId }
                });

                return RedirectToAction(nameof(NewOrder), new { id = scriptId, orderId = orderResult.Id });
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> DeleteFromBasket(int orderDetailId, int scriptId)
        {
            try
            {
                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(x => x.Id.Equals(orderDetailId));
                if (orderDetail == null)
                    throw new Exception("order item not found.");
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction(nameof(NewOrder), new { id = scriptId });
        }
    }
}
