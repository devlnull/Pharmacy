using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Scripts;

namespace Pharmacy.Services.ScriptService
{
    public class ScriptService : IScriptService
    {
        private AppDbContext _context;
        private AppUserManager _userManager;
        private IHttpContextAccessor _httpContextAccessor;
        private IUserService _userService;

        public ScriptService(AppDbContext context, AppUserManager userManager,
            IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
            _userService = userService ?? throw new ArgumentException(nameof(userService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentException(nameof(httpContextAccessor));
        }

        public async Task RequestNewScript(RequestScriptModel model)
        {
            try
            {
                var patient = await _userService.GetCurrentUserAsync<Patient>(UserTypes.Patient) as Patient;
                if (patient == null)
                    throw new Exception("user is not a patient");
                Script script = new Script()
                {
                    Status = ScriptStatuses.Pending,
                    PatientId = patient.Id,
                    Request = model.Request,
                    DoctorId = model.Doctor
                };
                await _context.Scripts.AddAsync(script);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task RespondToScript(RespondToScriptModel model)
        {
            try
            {
                var doc = await _userService.GetCurrentUserAsync<Doctor>(UserTypes.Doctor) as Doctor;
                if (doc == null)
                    throw new Exception("user is not a doctor");
                Script script = await _context.Scripts.FindAsync(model.Id);
                List<ScriptDetail> details = new List<ScriptDetail>();
                var medicines = model.Medicines.Split(',').ToList();
                medicines.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                var medicineIds = medicines.Select(x => Convert.ToInt32(x));
                foreach (var medId in medicineIds)
                {
                    ScriptDetail detail = new ScriptDetail()
                    {
                        MedicineId = medId,
                        ScriptId = script.Id,
                    };
                    details.Add(detail);
                }
                if (details.Any())
                {
                    script.Status = ScriptStatuses.Responded;
                    script.DoctorId = doc.Id;
                    script.Response = model.Response;
                    await _context.ScriptDetails.AddRangeAsync(details);
                    _context.Scripts.Update(script);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
