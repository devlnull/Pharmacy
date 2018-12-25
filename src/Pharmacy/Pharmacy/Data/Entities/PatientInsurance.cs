namespace Pharmacy.Data.Entities
{
    public class PatientInsurance : Entity
    {
        public virtual Patient Patient { get; set; }
        public int PatientId { get; set; }
        public virtual Insurance Insurance { get; set; }
        public int InsuranceId { get; set; }
    }
}
