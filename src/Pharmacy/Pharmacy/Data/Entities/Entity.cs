using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.Data.Entities
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
