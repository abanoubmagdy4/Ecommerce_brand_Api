namespace Ecommerce_brand_Api.Models.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be positive")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount cannot be negative")]
        public decimal Discount { get; set; }
        public bool IsDeleted { get; set; } = false;
        public decimal TotalPrice { get; set; }
    }
}
