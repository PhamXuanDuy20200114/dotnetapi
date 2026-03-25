using System.ComponentModel.DataAnnotations;

namespace Api.Models.Product
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; } = false;

        public Product Product { get; set; } = null!;
        public ProductSize? Size { get; set; }
        public ProductColor? Color { get; set; }
    }
}
