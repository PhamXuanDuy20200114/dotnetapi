namespace Api.DTOs.Discount
{
    public class DiscountCreateDto
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Type { get; set; } = "percent";
        public decimal Value { get; set; }
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool RequireCode { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProductId { get; set; }
    }

    public class DiscountApplyDto
    {
        public string Code { get; set; } = null!;
        public decimal OrderValue { get; set; }
        public int? ProductId { get; set; }
    }

    public class DiscountResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Type { get; set; } = null!;
        public decimal Value { get; set; }
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public bool IsActive { get; set; }
        public bool RequireCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
    }

    public class DiscountApplyResultDto
    {
        public bool Valid { get; set; }
        public string? Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
