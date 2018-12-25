using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data.Entities;
using System;

namespace Pharmacy.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorLicense> DoctorLicenses { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<InsuranceSupport> InsuranceSupports { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineCategory> MedicineCategories { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientInsurance> PatientInsurances { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Script> Scripts { get; set; }
        public DbSet<ScriptDetail> ScriptDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Script>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Scripts).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Orders).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.ScriptDetail)
                .WithOne(x => x.OrderDetail).OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }
}
