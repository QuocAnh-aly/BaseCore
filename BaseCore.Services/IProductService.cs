using BaseCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCore.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);   // phải là Task
        Task DeleteProductAsync(int id);

        Task<(List<Product> Products, int TotalCount)> SearchAsync(
            string keyword,
            int? categoryId,
            int page,
            int pageSize);
    }
}
