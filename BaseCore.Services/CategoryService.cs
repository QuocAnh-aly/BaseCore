using BaseCore.Entities;
using BaseCore.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly MySqlDbContext _context;

        public CategoryService(MySqlDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET BY ID
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // CREATE
        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        // UPDATE
        public async Task UpdateAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);

            if (existing != null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;

                _context.Categories.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}