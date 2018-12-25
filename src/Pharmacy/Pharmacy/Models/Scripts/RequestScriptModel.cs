using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Scripts
{
    public class RequestScriptModel
    {
        public int Id { get; set; }
        public int Doctor { get; set; }
        public int Patient { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(1024, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Request")]
        public string Request { get; set; }
    }
}
