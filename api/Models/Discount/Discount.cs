using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Discount
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        // "percent" hoặc "fixed"
        public string Type { get; set; } = "percent";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscount { get; set; }

        public bool IsActive { get; set; } = true;
        public bool RequireCode { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // null = áp dụng toàn bộ, có giá trị = chỉ áp dụng cho sản phẩm đó
        public int? ProductId { get; set; }
    }
}
