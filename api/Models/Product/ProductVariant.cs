using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Product
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public int? TypeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product Product { get; set; } = null!;
        public ProductSize? Size { get; set; }
        public ProductColor? Color { get; set; }
        public ProductType? Type { get; set; }
    }
}
