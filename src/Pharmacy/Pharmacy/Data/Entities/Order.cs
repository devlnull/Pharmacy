using System.Collections.Generic;

namespace Pharmacy.Data.Entities
{
    public class Order : Entity
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        
        public virtual Patient Patient { get; set; }
        public int PatientId { get; set; }
        public virtual Script Script { get; set; }
        public int ScriptId { get; set; }
        public OrderStatuses Status { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
