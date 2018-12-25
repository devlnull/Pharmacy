using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Products
{
    public class CompanyProductModel
    {
        public int Company { get; set; }
        public int Medicine { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public double Price { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "ExpireDate")]
        public DateTime ExpireDate { get; set; } = DateTime.Now;
    }
}
