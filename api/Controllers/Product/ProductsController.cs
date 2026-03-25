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
    public class ProductsController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly ICacheService _cache;
        public ProductsController(DBContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string key = "products:all";
            var cached = await _cache.GetAsync<List<ProductResponseDto>>(key);
            if (cached != null) return Ok(cached);

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Types)
                .Include(p => p.Variants).ThenInclude(v => v.Size)
                .Include(p => p.Variants).ThenInclude(v => v.Color)
                .Include(p => p.Variants).ThenInclude(v => v.Type)
                .Include(p => p.Images)
                .Select(p => ToResponse(p))
                .ToListAsync();

            await _cache.SetAsync(key, products, TimeSpan.FromMinutes(5));
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var key = $"products:{id}";
            var cached = await _cache.GetAsync<ProductResponseDto>(key);
            if (cached != null) return Ok(cached);

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Types)
                .Include(p => p.Variants).ThenInclude(v => v.Size)
                .Include(p => p.Variants).ThenInclude(v => v.Color)
                .Include(p => p.Variants).ThenInclude(v => v.Type)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var response = ToResponse(product);
            await _cache.SetAsync(key, response, TimeSpan.FromMinutes(5));
            return Ok(response);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var key = $"products:category:{categoryId}";
            var cached = await _cache.GetAsync<List<ProductResponseDto>>(key);
            if (cached != null) return Ok(cached);

            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Types)
                .Include(p => p.Variants).ThenInclude(v => v.Size)
                .Include(p => p.Variants).ThenInclude(v => v.Color)
                .Include(p => p.Variants).ThenInclude(v => v.Type)
                .Include(p => p.Images)
                .Select(p => ToResponse(p))
                .ToListAsync();

            await _cache.SetAsync(key, products, TimeSpan.FromMinutes(5));
            return Ok(products);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var product = new Models.Product.Product
            {
                Name = dto.Name,
                Description = dto.Description,
                BasePrice = dto.BasePrice,
                CategoryId = dto.CategoryId,
                Sizes = dto.Sizes.Select(s => new Models.Product.ProductSize { Size = s.Size }).ToList(),
                Colors = dto.Colors.Select(c => new Models.Product.ProductColor { Color = c.Color }).ToList(),
                Types = dto.Types.Select(t => new Models.Product.ProductType { Name = t.Name, Description = t.Description }).ToList()
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            foreach (var v in dto.Variants)
                _context.ProductVariants.Add(new Models.Product.ProductVariant
                {
                    ProductId = product.Id,
                    SizeId = v.SizeId, ColorId = v.ColorId, TypeId = v.TypeId,
                    Price = v.Price, Stock = v.Stock
                });
            await _context.SaveChangesAsync();

            await InvalidateProductCache(product.Id, product.CategoryId);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, await GetProductResponse(product.Id));
        }

        [Authorize]
        [HttpPost("{id}/variants")]
        public async Task<IActionResult> AddVariant(int id, ProductVariantDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var variant = new Models.Product.ProductVariant
            {
                ProductId = id, SizeId = dto.SizeId, ColorId = dto.ColorId,
                TypeId = dto.TypeId, Price = dto.Price, Stock = dto.Stock
            };
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();
            await InvalidateProductCache(id, product.CategoryId);
            return Ok(variant);
        }

        [Authorize]
        [HttpPut("{id}/variants/{variantId}")]
        public async Task<IActionResult> UpdateVariant(int id, int variantId, ProductVariantDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId && v.ProductId == id);
            if (variant == null) return NotFound();
            variant.SizeId = dto.SizeId; variant.ColorId = dto.ColorId;
            variant.TypeId = dto.TypeId; variant.Price = dto.Price; variant.Stock = dto.Stock;
            await _context.SaveChangesAsync();

            var product = await _context.Products.FindAsync(id);
            await InvalidateProductCache(id, product!.CategoryId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddImage(int id, ProductImageDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (dto.IsMain)
                await _context.ProductImages
                    .Where(i => i.ProductId == id && i.IsMain)
                    .ExecuteUpdateAsync(i => i.SetProperty(x => x.IsMain, false));

            var image = new Models.Product.ProductImage
            {
                ProductId = id, ImageUrl = dto.ImageUrl,
                IsMain = dto.IsMain, SizeId = dto.SizeId, ColorId = dto.ColorId
            };
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            await InvalidateProductCache(id, product.CategoryId);
            return Ok(new ProductImageResponseDto
            {
                Id = image.Id, ImageUrl = image.ImageUrl,
                IsMain = image.IsMain, SizeId = image.SizeId, ColorId = image.ColorId
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCreateDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.Name = dto.Name; product.Description = dto.Description;
            product.BasePrice = dto.BasePrice; product.CategoryId = dto.CategoryId;
            await _context.SaveChangesAsync();
            await InvalidateProductCache(id, product.CategoryId);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            await InvalidateProductCache(id, product.CategoryId);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return NotFound();
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            await InvalidateProductCache(image.ProductId, null);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}/variants/{variantId}")]
        public async Task<IActionResult> DeleteVariant(int id, int variantId)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId && v.ProductId == id);
            if (variant == null) return NotFound();
            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(id);
            await InvalidateProductCache(id, product?.CategoryId);
            return NoContent();
        }

        private async Task InvalidateProductCache(int productId, int? categoryId)
        {
            await _cache.RemoveAsync("products:all");
            await _cache.RemoveAsync($"products:{productId}");
            if (categoryId.HasValue)
                await _cache.RemoveAsync($"products:category:{categoryId}");
        }

        private async Task<ProductResponseDto> GetProductResponse(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category).Include(p => p.Sizes).Include(p => p.Colors)
                .Include(p => p.Types)
                .Include(p => p.Variants).ThenInclude(v => v.Size)
                .Include(p => p.Variants).ThenInclude(v => v.Color)
                .Include(p => p.Variants).ThenInclude(v => v.Type)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            return ToResponse(product!);
        }

        private static ProductResponseDto ToResponse(Models.Product.Product p) => new()
        {
            Id = p.Id, Name = p.Name, Description = p.Description,
            BasePrice = p.BasePrice, Category = p.Category?.Name ?? "", CreatedAt = p.CreatedAt,
            Sizes = p.Sizes.Select(s => new ProductSizeResponseDto { Id = s.Id, Size = s.Size }).ToList(),
            Colors = p.Colors.Select(c => new ProductColorResponseDto { Id = c.Id, Color = c.Color }).ToList(),
            Types = p.Types.Select(t => new ProductTypeResponseDto { Id = t.Id, Name = t.Name, Description = t.Description }).ToList(),
            Variants = p.Variants.Select(v => new ProductVariantResponseDto
            {
                Id = v.Id, SizeId = v.SizeId, Size = v.Size?.Size,
                ColorId = v.ColorId, Color = v.Color?.Color,
                TypeId = v.TypeId, Type = v.Type?.Name,
                Price = v.Price, Stock = v.Stock
            }).ToList(),
            Images = p.Images.Select(i => new ProductImageResponseDto
            {
                Id = i.Id, ImageUrl = i.ImageUrl, IsMain = i.IsMain,
                SizeId = i.SizeId, ColorId = i.ColorId
            }).ToList()
        };
    }
}
