namespace Pharmacy.Models
{
    public class HomeModel
    {
        public bool? Active { get; set; }
        public int? AdminCount { get; set; }
        public int? DoctorCount { get; set; }
        public int? EmployeeCount { get; set; }
        public int? PatientCount { get; set; }
        public int? MedicineCount { get; set; }
        public int? InsuranceCount { get; set; }
        public int? MedicineCategoryCount { get; set; }
        public int? ProductCount { get; set; }
        public int? CompanyCount { get; set; }
        public int? OrderCount { get; set; }
        public int? ScriptCount { get; set; }
        public int? DoctorLicenses { get; set; }
        public int? CurrentDoctorLicenses { get; set; }
        public int? CurrentDoctorScripts { get; set; }
        public int? CurrentPatientScripts { get; set; }
        public int? CurrentPatientOrders { get; set; }
        public int? CurrentPatientBasketItems { get; set; }
    }
}
