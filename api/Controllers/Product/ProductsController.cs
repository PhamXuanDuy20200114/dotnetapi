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

            var productList = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Details).ThenInclude(d => d.Images)
                .ToListAsync();

            var discounts = await _context.Discounts.ToListAsync();
            var now = DateTime.UtcNow;
            Models.Discount.Discount? GetDiscount(int pid) => discounts
                .Where(d => d.IsActive && !d.RequireCode
                    && (d.ProductId == null || d.ProductId == pid)
                    && (d.StartDate == null || d.StartDate <= now)
                    && (d.EndDate == null || d.EndDate >= now))
                .OrderByDescending(d => d.ProductId.HasValue)
                .FirstOrDefault();

            var products = productList.Select(p => ToResponse(p, GetDiscount(p.Id))).ToList();
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
                .Include(p => p.Details).ThenInclude(d => d.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var discount = await GetActiveDiscount(id);
            var response = ToResponse(product, discount);
            await _cache.SetAsync(key, response, TimeSpan.FromMinutes(5));
            return Ok(response);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var key = $"products:category:{categoryId}";
            var cached = await _cache.GetAsync<List<ProductResponseDto>>(key);
            if (cached != null) return Ok(cached);

            var productList = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.Details).ThenInclude(d => d.Images)
                .ToListAsync();

            var discounts = await _context.Discounts.ToListAsync();
            var now = DateTime.UtcNow;
            Models.Discount.Discount? GetDiscount(int pid) => discounts
                .Where(d => d.IsActive && !d.RequireCode
                    && (d.ProductId == null || d.ProductId == pid)
                    && (d.StartDate == null || d.StartDate <= now)
                    && (d.EndDate == null || d.EndDate >= now))
                .OrderByDescending(d => d.ProductId.HasValue)
                .FirstOrDefault();

            var products = productList.Select(p => ToResponse(p, GetDiscount(p.Id))).ToList();
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
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await InvalidateProductCache(product.Id, product.CategoryId);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, await GetProductResponse(product.Id));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCreateDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.BasePrice = dto.BasePrice;
            product.CategoryId = dto.CategoryId;
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

        // ── ProductDetail ──────────────────────────────────────────

        [Authorize]
        [HttpPost("{id}/details")]
        public async Task<IActionResult> AddDetail(int id, ProductDetailDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var detail = new Models.Product.ProductDetail
            {
                ProductId = id, Name = dto.Name, Description = dto.Description,
                Size = dto.Size, Color = dto.Color, CostPrice = dto.CostPrice, Price = dto.Price, Stock = dto.Stock
            };
            _context.ProductDetails.Add(detail);
            await _context.SaveChangesAsync();
            await InvalidateProductCache(id, product.CategoryId);
            var discount = await GetActiveDiscount(id);
            return Ok(ToDetailResponse(detail, discount));
        }

        [Authorize]
        [HttpPut("{id}/details/{detailId}")]
        public async Task<IActionResult> UpdateDetail(int id, int detailId, ProductDetailDto dto)
        {
            var detail = await _context.ProductDetails.FirstOrDefaultAsync(d => d.Id == detailId && d.ProductId == id);
            if (detail == null) return NotFound();
            detail.Name = dto.Name; detail.Description = dto.Description;
            detail.Size = dto.Size; detail.Color = dto.Color;
            detail.CostPrice = dto.CostPrice; detail.Price = dto.Price; detail.Stock = dto.Stock;
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(id);
            await InvalidateProductCache(id, product!.CategoryId);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}/details/{detailId}")]
        public async Task<IActionResult> DeleteDetail(int id, int detailId)
        {
            var detail = await _context.ProductDetails.FirstOrDefaultAsync(d => d.Id == detailId && d.ProductId == id);
            if (detail == null) return NotFound();
            _context.ProductDetails.Remove(detail);
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(id);
            await InvalidateProductCache(id, product?.CategoryId);
            return NoContent();
        }

        // ── ProductImage ───────────────────────────────────────────

        [Authorize]
        [HttpPost("{id}/details/{detailId}/images")]
        public async Task<IActionResult> AddImage(int id, int detailId, ProductImageDto dto)
        {
            var detail = await _context.ProductDetails
                .Include(d => d.Images)
                .FirstOrDefaultAsync(d => d.Id == detailId && d.ProductId == id);
            if (detail == null) return NotFound();

            if (dto.IsMain)
                await _context.ProductImages
                    .Where(i => i.ProductDetailId == detailId && i.IsMain)
                    .ExecuteUpdateAsync(i => i.SetProperty(x => x.IsMain, false));

            var image = new Models.Product.ProductImage
            {
                ProductDetailId = detailId, ImageUrl = dto.ImageUrl, IsMain = dto.IsMain
            };
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(id);
            await InvalidateProductCache(id, product?.CategoryId);
            return Ok(new ProductImageResponseDto { Id = image.Id, ImageUrl = image.ImageUrl, IsMain = image.IsMain });
        }

        [Authorize]
        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return NotFound();
            var detail = await _context.ProductDetails.FindAsync(image.ProductDetailId);
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            if (detail != null) await InvalidateProductCache(detail.ProductId, null);
            return NoContent();
        }

        // ── Helpers ────────────────────────────────────────────────

        private async Task<Models.Discount.Discount?> GetActiveDiscount(int productId)
        {
            var now = DateTime.UtcNow;
            return await _context.Discounts
                .Where(d => d.IsActive && !d.RequireCode
                    && (d.ProductId == null || d.ProductId == productId)
                    && (d.StartDate == null || d.StartDate <= now)
                    && (d.EndDate == null || d.EndDate >= now))
                .OrderByDescending(d => d.ProductId.HasValue) // ưu tiên discount riêng cho product
                .FirstOrDefaultAsync();
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
                .Include(p => p.Category)
                .Include(p => p.Details).ThenInclude(d => d.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            var discount = await GetActiveDiscount(id);
            return ToResponse(product!, discount);
        }

        private static decimal ApplyDiscount(decimal basePrice, Models.Discount.Discount? d)
        {
            if (d == null) return basePrice;
            var now = DateTime.UtcNow;
            if (!d.IsActive || d.RequireCode) return basePrice;
            if (d.StartDate.HasValue && now < d.StartDate) return basePrice;
            if (d.EndDate.HasValue && now > d.EndDate) return basePrice;
            var discount = d.Type == "percent"
                ? basePrice * d.Value / 100
                : d.Value;
            if (d.MaxDiscount.HasValue && discount > d.MaxDiscount.Value) discount = d.MaxDiscount.Value;
            return Math.Max(0, basePrice - discount);
        }

        private static ProductDetailResponseDto ToDetailResponse(Models.Product.ProductDetail d, Models.Discount.Discount? discount)
        {
            var baseForDetail = d.Price ?? d.Product?.BasePrice ?? 0;
            return new()
            {
                Id = d.Id, Name = d.Name, Description = d.Description,
                Size = d.Size, Color = d.Color, CostPrice = d.CostPrice, Price = d.Price,
                SalePrice = ApplyDiscount(baseForDetail, discount),
                Stock = d.Stock,
                Images = d.Images.Select(i => new ProductImageResponseDto { Id = i.Id, ImageUrl = i.ImageUrl, IsMain = i.IsMain }).ToList()
            };
        }

        private static ProductResponseDto ToResponse(Models.Product.Product p, Models.Discount.Discount? discount) => new()
        {
            Id = p.Id, Name = p.Name, Description = p.Description,
            BasePrice = p.BasePrice, SalePrice = ApplyDiscount(p.BasePrice, discount),
            CategoryId = p.CategoryId, Category = p.Category?.Name ?? "", CreatedAt = p.CreatedAt,
            Details = p.Details.Select(d => ToDetailResponse(d, discount)).ToList()
        };
    }
}
