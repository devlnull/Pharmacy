using System;
using System.Collections.Generic;

namespace Pharmacy.Models.Orders
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int ScriptId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineDosage { get; set; }
        public string CompanyName { get; set; }
        public double ProductPrice { get; set; }
        public int ProductExistence { get; set; }
        public DateTime ProductExpireDate { get; set; }
        public string Insurance { get; set; }
        public bool SupportByInsurance { get; set; }
    }

    public class OrderListModel
    {
        public int OrderId { get; set; }
        public List<OrderModel> Products { get; set; }
        public List<OrderModel> Basket { get; set; }
    }

}
