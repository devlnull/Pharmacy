using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Medicines
{
    public class MedicineModel
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Dosage")]
        public string Dosage { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Category")]
        public int Category { get; set; }

        public int SupportedByInsurances { get; set; }
        public int ProducedByCompanies { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
