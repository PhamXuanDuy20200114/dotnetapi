using System.ComponentModel.DataAnnotations;

namespace Api.Models.Product
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Product Product { get; set; } = null!;
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
