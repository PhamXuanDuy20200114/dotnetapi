using System.ComponentModel.DataAnnotations;

namespace Api.Models.Product
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
