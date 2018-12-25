using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.Utilities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using static System.DateTime;

namespace Pharmacy
{
    public class DatabaseBootstraper
    {
        private async Task SeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<AppDbContext>();
                var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var _userManager = scope.ServiceProvider.GetRequiredService<AppUserManager>();

                #region Roles
                var roles = typeof(PharmacyRoles).GetAllPublicConstantValues<string>();
                foreach(var role in roles)
                {
                    IdentityResult result = await _roleManager.CreateAsync(new AppRole(role));
                    if (result.Succeeded)
                        Console.WriteLine($"Role with name {role} has been created.");
                    else
                        Console.WriteLine($"Could not create role with name {role}.");
                }
                #endregion

                #region Admin
                AppUser admin = new AppUser()
                {
                    UserName = "admin@pharmacy.com",
                    Email = "admin@pharmacy.com",
                    PhoneNumber = "09168086844"
                };
                IdentityResult adminResult = await _userManager.CreateAsync(admin, PharmacyDefaults.Password);
                if (adminResult.Succeeded)
                    Console.WriteLine("Admin has been successfully created.");
                else
                    Console.WriteLine("Could not create the Admin");
                adminResult = await _userManager.AddToRoleAsync(admin, PharmacyRoles.Admin);
                if (adminResult.Succeeded)
                    Console.WriteLine("Admin role has been successfully asigned to admin.");
                else
                    Console.WriteLine("Could not asign Admin role to user admin");
                #endregion

            }
        }

        public async Task BootstrapDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<AppDbContext>();
                          
                Task creatorTask = Task.Run(async () => 
                {
                    bool exist = false;
                    var relationalDatabaseCreator = _context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                    if (relationalDatabaseCreator != null)
                        exist = await relationalDatabaseCreator.ExistsAsync();
                    if (!exist)
                    {
                        bool ensureCreated = await _context.Database.EnsureCreatedAsync();
                        if (ensureCreated)
                        {
                            _context.Database.Migrate();
                        }
                    }
                });
                
                await creatorTask.ContinueWith(async (act) => 
                {
                    await SeedData(serviceProvider);
                });

                creatorTask.Start();
            }
        }
    }
}