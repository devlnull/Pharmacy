using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Entities
{
    public class Person : Entity
    {
        [Required]
        [MaxLength(64)]
        public string Firstname { get; set; }
        [Required]
        [MaxLength(64)]
        public string Lastname { get; set; }
        public DateTime Birthday { get; set; }
        public virtual Address Address { get; set; }
        public int AddressId { get; set; }
    }
}
