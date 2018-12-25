using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class MedicineCategory : Entity
    {
        public MedicineCategory()
        {
            Medicines = new HashSet<Medicine>();
        }
        
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public virtual ICollection<Medicine> Medicines { get; set; }
    }
}
