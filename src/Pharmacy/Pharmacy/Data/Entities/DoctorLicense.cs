using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class DoctorLicense : Entity
    {
        public virtual Doctor Doctor { get; set; }
        public int DoctorId { get; set; }
        [Required]
        [MaxLength(64)]
        public string Title { get; set; }
        [Required]
        public DoctorLicenseStatuses Status { get; set; }
    }
}
