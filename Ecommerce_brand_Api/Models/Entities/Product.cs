namespace Ecommerce_brand_Api.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<ProductImagesPaths> ProductImagesPaths { get; set; } = new List<ProductImagesPaths>();

        public decimal DiscountPercentage { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public double AverageRating { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

    }
}
