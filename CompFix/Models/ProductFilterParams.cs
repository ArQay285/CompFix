namespace CompFix.Models
{
    public class ProductFilterParams
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public int? Condition { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? HasDiscount { get; set; }
        public string? StockStatus { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
    }
}
