using BaseCore.Entities;
using BaseCore.Repository;
using Microsoft.EntityFrameworkCore;

namespace BaseCore.Services
{
    public class ProductService : IProductService
    {
        private readonly MySqlDbContext _context;

        public ProductService(MySqlDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // ================= GET BY ID =================
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ================= CREATE =================
        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // ================= UPDATE =================
        public async Task UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);

            if (existing == null) return;

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.Description = product.Description;
            existing.ImageUrl = product.ImageUrl;
            existing.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();
        }

        // ================= DELETE =================
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // ================= SEARCH + PAGING (FIXED) =================
        public async Task<(List<Product> Products, int TotalCount)> SearchAsync(
            string keyword,
            int? categoryId,
            int page,
            int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // FILTER KEYWORD
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));
            }

            // FILTER CATEGORY
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // TOTAL COUNT
            var totalCount = await query.CountAsync();

            // PAGING + SORT
            var products = await query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}