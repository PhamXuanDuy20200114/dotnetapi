namespace Api.DTOs.Product
{
    public class CategoryDto
    {
        public string Name { get; set; } = null!;
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }
        public List<ProductSizeDto> Sizes { get; set; } = new();
        public List<ProductColorDto> Colors { get; set; } = new();
        public List<ProductTypeDto> Types { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
    }

    public class ProductSizeDto
    {
        public string Size { get; set; } = null!;
    }

    public class ProductColorDto
    {
        public string Color { get; set; } = null!;
    }

    public class ProductTypeDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class ProductVariantDto
    {
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public int? TypeId { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string Category { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<ProductSizeResponseDto> Sizes { get; set; } = new();
        public List<ProductColorResponseDto> Colors { get; set; } = new();
        public List<ProductTypeResponseDto> Types { get; set; } = new();
        public List<ProductVariantResponseDto> Variants { get; set; } = new();
        public List<ProductImageResponseDto> Images { get; set; } = new();
    }

    public class ProductSizeResponseDto
    {
        public int Id { get; set; }
        public string Size { get; set; } = null!;
    }

    public class ProductColorResponseDto
    {
        public int Id { get; set; }
        public string Color { get; set; } = null!;
    }

    public class ProductTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class ProductVariantResponseDto
    {
        public int Id { get; set; }
        public int? SizeId { get; set; }
        public string? Size { get; set; }
        public int? ColorId { get; set; }
        public string? Color { get; set; }
        public int? TypeId { get; set; }
        public string? Type { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class ProductImageResponseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
    }
}
