using Pharmacy.Data;
using Pharmacy.Data.Entities;
using Pharmacy.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Services.ProductService
{
    public class ProductService : IProductService
    {
        private AppDbContext _context;
        private AppUserManager _userManager;

        public ProductService(AppDbContext context, AppUserManager userManager)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
        }

        public async Task AddCompanyProductAsync(CompanyProductModel model)
        {
            try
            {
                var company = await _context.Companies.FindAsync(model.Company);
                if (company == null)
                    throw new Exception("Company not found");
                var medicine = await _context.Medicines.FindAsync(model.Medicine);
                if (medicine == null)
                    throw new Exception("Medicine not found");
                Product product = new Product();
                product.Company = company;
                product.Medicine = medicine;
                product.Quantity = model.Quantity;
                product.Price = model.Price;
                product.CreateDate = model.CreateDate;
                product.ExpireDate = model.ExpireDate;
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
