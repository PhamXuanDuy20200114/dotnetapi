using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Product
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Category Category { get; set; } = null!;
        public ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();
        public ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();
        public ICollection<ProductType> Types { get; set; } = new List<ProductType>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
