using System.Collections.Generic;

namespace Pharmacy.Models.Orders
{
    public class NewOrderModel
    {
        public int Script { get; set; }
        public int Patient { get; set; }
        public List<int> Products { get; set; }
    }
}
