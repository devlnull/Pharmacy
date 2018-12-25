using Pharmacy.Data;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class SearchModel
    {
        [DataType(DataType.Text)]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        [Display(Name = "Query")]
        public string Query { get; set; } = null;
        [Display(Name = "Only Existence")]
        public bool OnlyExistence { get; set; }
        [Display(Name = "DoctorStatus")]
        public DoctorLicenseStatuses DoctorLicenseStatus { get; set; } = DoctorLicenseStatuses.Pending;
    }
}
