using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class Address : Entity
    {
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
