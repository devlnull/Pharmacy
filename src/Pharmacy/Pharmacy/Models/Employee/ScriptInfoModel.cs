using Pharmacy.Data;

namespace Pharmacy.Models.Employee
{
    public class ScriptInfoModel
    {
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public ScriptStatuses ScriptStatus { get; set; }
        public string OrderStatus { get; set; }
        public int MedicineScripted { get; set; }
    }
}
