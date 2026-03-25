using System.ComponentModel.DataAnnotations;

namespace Api.Models.Product
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public int ProductDetailId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; } = false;

        public ProductDetail ProductDetail { get; set; } = null!;
    }
}
