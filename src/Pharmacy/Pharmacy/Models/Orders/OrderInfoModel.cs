using Pharmacy.Data;

namespace Pharmacy.Models.Orders
{
    public class OrderInfoModel
    {
        public OrderStatuses Status { get; set; }
        public double TotalPrice { get; set; }
    }
}
