namespace Pharmacy.Data.Entities
{
    public class InsuranceSupport : Entity
    {
        public virtual Insurance Insurance { get; set; }
        public int InsuranceId { get; set; }
        public virtual Medicine Medicine { get; set; }
        public int MedicineId { get; set; }
    }
}
