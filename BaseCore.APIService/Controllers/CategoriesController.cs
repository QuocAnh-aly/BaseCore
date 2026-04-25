using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;
using System.Threading.Tasks;

namespace BaseCore.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepositoryEF _categoryRepository;

        public CategoriesController(ICategoryRepositoryEF categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CategoryDto dto)
        {
            var existing = await _categoryRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                return BadRequest(new { message = "Category name already exists" });

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description ?? ""
            };

            await _categoryRepository.AddAsync(category);

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            category.Name = dto.Name ?? category.Name;
            category.Description = dto.Description ?? category.Description;

            await _categoryRepository.UpdateAsync(category);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            await _categoryRepository.DeleteAsync(category);
            return Ok(new { message = "Category deleted successfully" });
        }
    }

    public class CategoryDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}