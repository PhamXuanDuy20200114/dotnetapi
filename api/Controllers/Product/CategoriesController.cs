using Api.Data;
using Api.DTOs.Product;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly ICacheService _cache;
        public CategoriesController(DBContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string key = "categories:all";
            var cached = await _cache.GetAsync<List<CategoryResponseDto>>(key);
            if (cached != null) return Ok(cached);

            var categories = await _context.Categories
                .Select(c => new CategoryResponseDto { Id = c.Id, Name = c.Name })
                .ToListAsync();

            await _cache.SetAsync(key, categories, TimeSpan.FromMinutes(10));
            return Ok(categories);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var category = new Models.Product.Category { Name = dto.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("categories:all");
            return Ok(new CategoryResponseDto { Id = category.Id, Name = category.Name });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("categories:all");
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("categories:all");
            return NoContent();
        }
    }
}
