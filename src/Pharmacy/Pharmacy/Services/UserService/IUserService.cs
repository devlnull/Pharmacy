using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Users;
using System.Threading.Tasks;

namespace Pharmacy.Services
{
    public interface IUserService
    {
        Task<Doctor> RegisterDoctorAsync(Pharmacy.Areas.Identity.Pages.Account.RegisterModel.InputModel model, int userId);
        Task<Patient> RegisterPatientAsync(Pharmacy.Areas.Identity.Pages.Account.RegisterModel.InputModel model, int userId);
        Task<Employee> RegisterEmployeeAsync(Pharmacy.Areas.Identity.Pages.Account.RegisterModel.InputModel model, int userId);
        Task RegisterAdminAsync(RegisterAdminModel model);
        Task ActiveDoctorAsync(string username);
        Task ActiveEmployeeAsync(string username);
        Task DeactiveDoctorAsync(string username);
        Task DeactiveEmployeeAsync(string username);
        Task<Person> GetCurrentUserAsync<T>(UserTypes userType);
    }
}
