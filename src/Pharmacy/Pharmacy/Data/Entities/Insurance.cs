using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class Insurance : Entity
    {
        public Insurance()
        {
            Supports = new HashSet<InsuranceSupport>();
        }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public ICollection<InsuranceSupport> Supports { get; set; }
    }
}
