namespace Api.DTOs.Order
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int? ProductDetailId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? VariantName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderCreateDto
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? CustomerEmail { get; set; }
        public string Address { get; set; } = null!;
        public string? Note { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderUpdateStatusDto
    {
        public string Status { get; set; } = null!;
    }

    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductDetailId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? VariantName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? CustomerEmail { get; set; }
        public string Address { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Note { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
