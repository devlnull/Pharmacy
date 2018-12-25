using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Users
{
    public class ProfileModel
    {
        [Required]
        [MaxLength(64)]
        public string Firstname { get; set; }
        [Required]
        [MaxLength(64)]
        public string Lastname { get; set; }
        [Required]
        [MaxLength(13)]
        public string PhoneNumber { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [MaxLength(64)]
        public string Country { get; set; }
        [MaxLength(64)]
        public string City { get; set; }
        [MaxLength(64)]
        public string Neighborhood { get; set; }
        [MaxLength(512)]
        public string Line1 { get; set; }
        [MaxLength(512)]
        public string Line2 { get; set; }
    }
}