using Pharmacy.Models.InsuranceSupport;
using System.Threading.Tasks;

namespace Pharmacy.Services.InsuranceService
{
    public interface IInsuranceSupportService
    {
        Task SupportMedicineAsync(InsuranceSupportModel model);
    }
}
