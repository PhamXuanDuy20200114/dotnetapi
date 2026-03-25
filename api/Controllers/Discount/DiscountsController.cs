using Api.Data;
using Api.DTOs.Discount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Discount
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiscountsController : ControllerBase
    {
        private readonly DBContext _context;
        public DiscountsController(DBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var discounts = await _context.Discounts.OrderByDescending(d => d.CreatedAt).ToListAsync();
            var productIds = discounts.Where(d => d.ProductId.HasValue).Select(d => d.ProductId!.Value).Distinct().ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Name);
            return Ok(discounts.Select(d => ToResponse(d, products)));
        }

        // Lấy các discount tự động (không cần nhập mã) cho một đơn hàng
        [HttpPost("auto")]
        public async Task<IActionResult> GetAutoDiscounts(DiscountApplyDto dto)
        {
            var now = DateTime.UtcNow;
            var autoDiscounts = await _context.Discounts.Where(d =>
                !d.RequireCode && d.IsActive &&
                (d.StartDate == null || d.StartDate <= now) &&
                (d.EndDate == null || d.EndDate >= now) &&
                (d.ProductId == null || d.ProductId == dto.ProductId) &&
                (d.MinOrderValue == null || d.MinOrderValue <= dto.OrderValue)
            ).ToListAsync();

            var results = autoDiscounts.Select(d =>
            {
                decimal amount = d.Type == "percent" ? dto.OrderValue * d.Value / 100 : d.Value;
                if (d.MaxDiscount.HasValue && amount > d.MaxDiscount) amount = d.MaxDiscount.Value;
                amount = Math.Min(amount, dto.OrderValue);
                return new { d.Id, d.Name, d.Type, d.Value, DiscountAmount = amount };
            }).ToList();

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var d = await _context.Discounts.FindAsync(id);
            if (d == null) return NotFound();
            var productName = d.ProductId.HasValue ? (await _context.Products.FindAsync(d.ProductId))?.Name : null;
            return Ok(ToResponse(d, d.ProductId.HasValue && productName != null ? new() { { d.ProductId.Value, productName } } : new()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiscountCreateDto dto)
        {
            if (dto.RequireCode && await _context.Discounts.AnyAsync(d => d.Code == dto.Code.ToUpper()))
                return BadRequest("Mã giảm giá đã tồn tại");

            var discount = new Models.Discount.Discount
            {
                Name = dto.Name, Code = dto.RequireCode ? dto.Code.ToUpper() : "", Type = dto.Type,
                Value = dto.Value, MinOrderValue = dto.MinOrderValue, MaxDiscount = dto.MaxDiscount,
                IsActive = dto.IsActive, RequireCode = dto.RequireCode,
                StartDate = dto.StartDate, EndDate = dto.EndDate, ProductId = dto.ProductId
            };
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return Ok(ToResponse(discount, new()));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiscountCreateDto dto)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();
            if (dto.RequireCode && await _context.Discounts.AnyAsync(d => d.Code == dto.Code.ToUpper() && d.Id != id))
                return BadRequest("Mã giảm giá đã tồn tại");

            discount.Name = dto.Name; discount.Code = dto.RequireCode ? dto.Code.ToUpper() : "";
            discount.Type = dto.Type; discount.Value = dto.Value;
            discount.MinOrderValue = dto.MinOrderValue; discount.MaxDiscount = dto.MaxDiscount;
            discount.IsActive = dto.IsActive; discount.RequireCode = dto.RequireCode;
            discount.StartDate = dto.StartDate; discount.EndDate = dto.EndDate; discount.ProductId = dto.ProductId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(DiscountApplyDto dto)
        {
            var now = DateTime.UtcNow;
            var discount = await _context.Discounts.FirstOrDefaultAsync(d =>
                d.RequireCode && d.Code == dto.Code.ToUpper() && d.IsActive &&
                (d.StartDate == null || d.StartDate <= now) &&
                (d.EndDate == null || d.EndDate >= now));

            if (discount == null)
                return Ok(new DiscountApplyResultDto { Valid = false, Message = "Mã giảm giá không hợp lệ hoặc đã hết hạn" });

            if (discount.ProductId.HasValue && discount.ProductId != dto.ProductId)
                return Ok(new DiscountApplyResultDto { Valid = false, Message = "Mã giảm giá không áp dụng cho sản phẩm này" });

            if (discount.MinOrderValue.HasValue && dto.OrderValue < discount.MinOrderValue)
                return Ok(new DiscountApplyResultDto { Valid = false, Message = $"Đơn hàng tối thiểu {discount.MinOrderValue:N0}₫" });

            decimal discountAmount = discount.Type == "percent"
                ? dto.OrderValue * discount.Value / 100
                : discount.Value;

            if (discount.MaxDiscount.HasValue && discountAmount > discount.MaxDiscount)
                discountAmount = discount.MaxDiscount.Value;

            discountAmount = Math.Min(discountAmount, dto.OrderValue);

            return Ok(new DiscountApplyResultDto { Valid = true, DiscountAmount = discountAmount, FinalPrice = dto.OrderValue - discountAmount });
        }

        private static DiscountResponseDto ToResponse(Models.Discount.Discount d, Dictionary<int, string> products) => new()
        {
            Id = d.Id, Name = d.Name, Code = d.Code, Type = d.Type, Value = d.Value,
            MinOrderValue = d.MinOrderValue, MaxDiscount = d.MaxDiscount,
            IsActive = d.IsActive, RequireCode = d.RequireCode,
            StartDate = d.StartDate, EndDate = d.EndDate,
            CreatedAt = d.CreatedAt, ProductId = d.ProductId,
            ProductName = d.ProductId.HasValue && products.TryGetValue(d.ProductId.Value, out var name) ? name : null
        };
    }
}
