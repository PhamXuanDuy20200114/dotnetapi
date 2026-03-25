using System.ComponentModel.DataAnnotations;

namespace Api.Models.Product
{
    public class ProductColor
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Color { get; set; } = null!;

        public Product Product { get; set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
