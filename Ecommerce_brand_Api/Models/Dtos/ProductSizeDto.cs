namespace Ecommerce_brand_Api.Models.Dtos
{
    public class ProductSizeDto
    {
        public int Id { get; set; }
        public string Size { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int StockQuantity { get; set; } = 0;
    }
}
