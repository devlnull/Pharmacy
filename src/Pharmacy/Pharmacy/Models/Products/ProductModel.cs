using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Products
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Medicine { get; set; }
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
        public DateTime CreateDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "ExpireDate")]
        public DateTime ExpireDate { get; set; }
    }
}
