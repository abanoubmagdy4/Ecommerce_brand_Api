namespace Ecommerce_brand_Api.Models.Dtos
{

    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Image file name is required")]
        public List<IFormFile> Images { get; set; } = new();

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal DiscountPercentage { get; set; }

        [Required]
        public decimal PriceAfterDiscount
        {
            get
            {
                return Price - (Price * DiscountPercentage / 100);
            }
        }
        public bool IsDeleted { get; set; } = false;

        [Required]
        public int CategoryId { get; set; }
    }
}
