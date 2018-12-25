namespace Pharmacy.Data.Entities
{
    public class ScriptDetail : Entity
    {
        public virtual Script Script { get; set; }
        public int ScriptId { get; set; }
        public virtual Medicine Medicine { get; set; }
        public int MedicineId { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
