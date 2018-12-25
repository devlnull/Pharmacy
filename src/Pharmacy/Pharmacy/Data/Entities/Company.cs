using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class Company : Entity
    {
        public Company()
        {
            Products = new HashSet<Product>();
        }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
