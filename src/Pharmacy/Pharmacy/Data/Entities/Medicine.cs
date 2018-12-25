using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class Medicine : Entity
    {
        public Medicine()
        {
            InsuranceSupports = new HashSet<InsuranceSupport>();
        }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public virtual MedicineCategory Category { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(32)]
        public string Dosage { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<InsuranceSupport> InsuranceSupports { get; set; }
    }
}
