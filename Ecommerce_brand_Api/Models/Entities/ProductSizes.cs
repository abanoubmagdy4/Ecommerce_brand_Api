namespace Ecommerce_brand_Api.Models.Entities
{
    public class ProductSizes
    {
        public int Id { get; set; }
        public string Size { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int StockQuantity { get; set; } = 0;
        public Product Product { get; set; } = null!;
    }
}
