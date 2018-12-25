using Pharmacy.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Models.Doctors
{
    public class LicenseModel
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Title")]
        public string Title { get; set; }
        public string DoctorName { get; set; }
        public DoctorLicenseStatuses Status { get; set; }
    }
}