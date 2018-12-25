namespace Pharmacy.Data.Entities
{
    public class OrderDetail : Entity
    {
        public virtual Order Order { get; set; }
        public int OrderId { get; set; }
        public virtual ScriptDetail ScriptDetail { get; set; }
        public int ScriptDetailId { get; set; }
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }
        public OrderStatuses Status { get; set; }
    }
}
