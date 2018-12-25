using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Orders;

namespace Pharmacy.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IUserService _userService;

        public OrderService(AppDbContext context, AppUserManager userManager, IUserService userService)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
            _userService = userService ?? throw new ArgumentException(nameof(userService));
        }

        public async Task CheckOutAsync()
        {
            try
            {
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient.");
                var userPatient = await _context.Patients.Include(x => x.Orders).FirstOrDefaultAsync(x => x.Id.Equals(patient.Id));
                var userOrderIds = userPatient.Orders.Select(x => x.Id);
                var orderDetails = _context.OrderDetails.Where(x => userOrderIds.Contains(x.OrderId));
                foreach(var detail in orderDetails)
                {
                    detail.Status = OrderStatuses.Ordered;
                    _context.OrderDetails.Update(detail);
                }
                var orders = _context.Orders.Include(x => x.OrderDetails).Where(x => userOrderIds.Contains(x.Id));

                foreach(var order in userPatient.Orders)
                {
                    if(order.OrderDetails.All(x=>x.Status == OrderStatuses.Ordered))
                    {
                        order.Status = OrderStatuses.Ordered;
                    }
                    _context.Orders.Update(order);
                }
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> OrderNewItemAsync(NewOrderModel model)
        {
            try
            {
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient.");
                var script = await _context.Scripts.Include(x => x.ScriptDetails).FirstOrDefaultAsync(x => x.Id.Equals(model.Script));
                if (script == null)
                    throw new Exception("script not found.");
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.ScriptId.Equals(script.Id));
                if (order == null)
                { // order is new

                    #region submit new order
                    order = new Order()
                    {
                        PatientId = patient.Id,
                        ScriptId = script.Id,
                        Status = OrderStatuses.Preorder,
                    };
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();
                    #endregion
                }

                var products = await _context.Products.Include(x => x.Medicine).Where(x => model.Products.Contains(x.Id)).ToListAsync();
                if (!products.Any())
                    throw new Exception("There is not product for these medicines");
                var scriptDetails = await _context.ScriptDetails.Where(x => x.ScriptId.Equals(script.Id)).ToListAsync();
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                #region submit order details
                foreach (var item in products)
                {
                    if (item.Quantity <= 0)
                        continue;
                    var prdMedId = item.MedicineId;
                    var scriptDetailId = scriptDetails.FirstOrDefault(x => x.MedicineId.Equals(prdMedId));
                    if (scriptDetailId == null)
                        continue;
                    orderDetails.Add(new OrderDetail()
                    {
                        OrderId = order.Id,
                        ScriptDetailId = scriptDetailId.Id,
                        ProductId = item.Id,
                    });

                    //decrease the product quantity
                    item.Quantity = item.Quantity - 1;
                }
                await _context.OrderDetails.AddRangeAsync(orderDetails);
                await _context.SaveChangesAsync();
                #endregion
                return order;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
