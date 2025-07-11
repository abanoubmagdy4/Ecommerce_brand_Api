namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class OrderFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? StatusTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortDirection { get; set; } = "DESC";
    }
}
