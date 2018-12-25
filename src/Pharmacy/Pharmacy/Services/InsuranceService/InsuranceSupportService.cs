using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models.InsuranceSupport;
using System;
using System.Threading.Tasks;

namespace Pharmacy.Services.InsuranceService
{
    public class InsuranceSupportService : IInsuranceSupportService
    {
        private AppDbContext _context;
        private AppUserManager _userManager;

        public InsuranceSupportService(AppDbContext context, AppUserManager userManager)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
        }

        public async Task SupportMedicineAsync(InsuranceSupportModel model)
        {
            try
            {
                var insurance = await _context.Insurances.FindAsync(model.Insurance);
                if (insurance == null)
                    throw new Exception("Insurance not found");
                var medicine = await _context.Medicines.FindAsync(model.Medicine);
                if (medicine == null)
                    throw new Exception("Medicine not found");
                var alreadySupport = await _context.InsuranceSupports
                    .FirstOrDefaultAsync(x => x.MedicineId.Equals(medicine.Id) && x.InsuranceId.Equals(insurance.Id));
                if (alreadySupport != null)
                    throw new Exception("Medicine is already support.");
                await _context.InsuranceSupports.AddAsync(new Data.Entities.InsuranceSupport()
                {
                    Insurance = insurance,
                    Medicine = medicine,
                    InsuranceId = insurance.Id,
                    MedicineId = insurance.Id
                });
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
