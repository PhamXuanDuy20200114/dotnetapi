namespace Api.DTOs.Product
{
    public class CategoryDto { public string Name { get; set; } = null!; }
    public class CategoryResponseDto { public int Id { get; set; } public string Name { get; set; } = null!; }

    public class ProductCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductDetailDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }

    public class UpdateStockDto { public int Stock { get; set; } }

    public class ProductImageResponseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }

    public class ProductDetailResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal SalePrice { get; set; }
        public int Stock { get; set; }
        public List<ProductImageResponseDto> Images { get; set; } = new();
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<ProductDetailResponseDto> Details { get; set; } = new();
    }
}
