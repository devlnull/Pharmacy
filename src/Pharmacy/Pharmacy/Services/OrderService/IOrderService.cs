using Pharmacy.Data.Entities;
using Pharmacy.Models.Orders;
using System.Threading.Tasks;

namespace Pharmacy.Services.OrderService
{
    public interface IOrderService
    {
        Task<Order> OrderNewItemAsync(NewOrderModel model);
        Task CheckOutAsync();
    }
}
