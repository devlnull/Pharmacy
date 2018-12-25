using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.MedicineCategory
{
    public class MedicineCategoryModel
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public int Medicines { get; set; }
    }
}