using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Scripts
{
    public class RespondToScriptModel
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string PatientName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(1024, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Response text")]
        public string Response { get; set; }
        public string Medicines { get; set; }
    }
}
