using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Areas.Identity.Pages.Account;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Users;
using Pharmacy.Utilities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Services
{
    public class UserService : IUserService
    {
        private AppDbContext _context;
        private AppUserManager _userManager;
        private IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext context, AppUserManager userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentException(nameof(httpContextAccessor));
        }

        public async Task RegisterAdminAsync(RegisterAdminModel model)
        {
            try
            {
                if (!model.Password.ToLower().Equals(model.ConfirmPassword.ToLower()))
                    throw new Exception("Password and confirm password must be matched.");
                if ((await _userManager.FindByNameAsync(model.Email)) != null)
                    throw new Exception("Username already exist.");
                AppUser user = new AppUser();
                user.UserName = model.Email;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    throw new Exception(result.ErrorGathering());
                await AssignRoleToUser(user.Id, PharmacyRoles.Admin);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AssignRoleToUser(int userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new Exception("user not found.");
                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                    throw new Exception($"Could not assign role '{role}' to user '{user.UserName}'");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Doctor> RegisterDoctorAsync(RegisterModel.InputModel model, int userId)
        {
            try
            {
                Doctor doc = new Doctor();
                doc.Birthday = model.Birthday;
                doc.Firstname = model.Firstname;
                doc.Lastname = model.Lastname;
                doc.Birthday = model.Birthday;
                doc.UserId = userId;
                doc.State = UserStates.Deactive;
                Address address = new Address();
                address.Country = address.City =
                    address.Line1 = address.Line2 = address.Neighborhood = string.Empty;
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.Addresses.AddAsync(address);
                        await _context.SaveChangesAsync();
                        doc.Address = address;
                        await _context.Doctors.AddAsync(doc);
                        await _context.SaveChangesAsync();
                        await AssignRoleToUser(userId, PharmacyRoles.Doctor);
                        transaction.Commit();
                    }
                    catch (Exception innerEx)
                    {
                        transaction.Rollback();
                        throw innerEx;
                    }
                }

                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Employee> RegisterEmployeeAsync(RegisterModel.InputModel model, int userId)
        {
            try
            {
                Employee emp = new Employee();
                emp.Birthday = model.Birthday;
                emp.Firstname = model.Firstname;
                emp.Lastname = model.Lastname;
                emp.Birthday = model.Birthday;
                emp.UserId = userId;
                emp.State = UserStates.Deactive;
                Address address = new Address();
                address.Country = address.City =
                    address.Line1 = address.Line2 = address.Neighborhood = string.Empty;
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.Addresses.AddAsync(address);
                        await _context.SaveChangesAsync();
                        emp.Address = address;
                        await _context.Employees.AddAsync(emp);
                        await _context.SaveChangesAsync();
                        await AssignRoleToUser(userId, PharmacyRoles.Employee);
                        transaction.Commit();
                    }
                    catch(Exception innerEx)
                    {
                        transaction.Rollback();
                        throw innerEx;
                    }
                }
                return emp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Patient> RegisterPatientAsync(RegisterModel.InputModel model, int userId)
        {
            try
            {
                Patient patient = new Patient();
                patient.Birthday = model.Birthday;
                patient.Firstname = model.Firstname;
                patient.Lastname = model.Lastname;
                patient.Birthday = model.Birthday;
                patient.UserId = userId;
                Address address = new Address();
                address.Country = address.City =
                    address.Line1 = address.Line2 = address.Neighborhood = string.Empty;
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.Addresses.AddAsync(address);
                        await _context.SaveChangesAsync();
                        patient.Address = address;
                        await _context.Patients.AddAsync(patient);
                        await _context.SaveChangesAsync();
                        await AssignRoleToUser(userId, PharmacyRoles.Patient);
                        transaction.Commit();
                    }
                    catch (Exception innerEx)
                    {
                        transaction.Rollback();
                        throw innerEx;
                    }
                }
                return patient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ActiveDoctorAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    throw new Exception("user not found.");
                var doc = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                doc.State = UserStates.Active;
                _context.Doctors.Update(doc);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ActiveEmployeeAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    throw new Exception("user not found.");
                var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                emp.State = UserStates.Active;
                _context.Employees.Update(emp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeactiveDoctorAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    throw new Exception("user not found.");
                var doc = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                doc.State = UserStates.Deactive;
                _context.Doctors.Update(doc);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeactiveEmployeeAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    throw new Exception("user not found.");
                var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                emp.State = UserStates.Deactive;
                _context.Employees.Update(emp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Person> GetCurrentUserAsync<T>(UserTypes userType)
        {
            try
            {
                var username = _httpContextAccessor.HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    throw new Exception("user not found");
                switch (userType)
                {
                    case UserTypes.Employee:
                        {
                            var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                            if (emp == null)
                                throw new Exception("user not found");
                            return emp;
                        }
                    case UserTypes.Doctor:
                        {
                            var doc = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                            if (doc == null)
                                throw new Exception("user not found");
                            return doc;
                        }
                    case UserTypes.Patient:
                        {
                            var patinet = await _context.Patients.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                            if (patinet == null)
                                throw new Exception("user not found");
                            return patinet;
                        }
                    default:
                        throw new Exception("user not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
