using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class Product : Entity
    {
        [Range(0,  int.MaxValue)]
        public int Quantity { get; set; }
        public virtual Medicine Medicine { get; set; }
        public int MedicineId { get; set; }
        public virtual Company Company { get; set; }
        public int CompanyId { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
