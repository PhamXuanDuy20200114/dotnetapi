using Api.Data;
using Api.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly DBContext _context;
        public OrdersController(DBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => ToResponse(o))
                .ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return Ok(ToResponse(order));
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            var total = dto.Items.Sum(i => i.Price * i.Quantity);
            var order = new Models.Order.Order
            {
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                Address = dto.Address,
                Note = dto.Note,
                Total = total,
                Items = dto.Items.Select(i => new Models.Order.OrderItem
                {
                    ProductId = i.ProductId,
                    ProductDetailId = i.ProductDetailId,
                    ProductName = i.ProductName,
                    VariantName = i.VariantName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, ToResponse(order));
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderUpdateStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            order.Status = dto.Status;
            await _context.SaveChangesAsync();
            return Ok(new { order.Id, order.Status });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderCreateDto dto)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            order.CustomerName = dto.CustomerName;
            order.CustomerPhone = dto.CustomerPhone;
            order.CustomerEmail = dto.CustomerEmail;
            order.Address = dto.Address;
            order.Note = dto.Note;
            order.Total = dto.Items.Sum(i => i.Price * i.Quantity);
            _context.OrderItems.RemoveRange(order.Items);
            foreach (var i in dto.Items)
                order.Items.Add(new Models.Order.OrderItem { OrderId = id, ProductId = i.ProductId, ProductDetailId = i.ProductDetailId, ProductName = i.ProductName, VariantName = i.VariantName, Price = i.Price, Quantity = i.Quantity });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static OrderResponseDto ToResponse(Models.Order.Order o) => new()
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            CustomerPhone = o.CustomerPhone,
            CustomerEmail = o.CustomerEmail,
            Address = o.Address,
            Status = o.Status,
            Note = o.Note,
            Total = o.Total,
            CreatedAt = o.CreatedAt,
            Items = o.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id, ProductId = i.ProductId, ProductDetailId = i.ProductDetailId,
                ProductName = i.ProductName, VariantName = i.VariantName,
                Price = i.Price, Quantity = i.Quantity
            }).ToList()
        };
    }
}
