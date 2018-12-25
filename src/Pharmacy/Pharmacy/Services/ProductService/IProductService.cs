using Pharmacy.Models.Products;
using System.Threading.Tasks;

namespace Pharmacy.Services.ProductService
{
    public interface IProductService
    {
        Task AddCompanyProductAsync(CompanyProductModel model);
    }
}
