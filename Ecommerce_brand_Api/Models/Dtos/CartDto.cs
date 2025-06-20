namespace Ecommerce_brand_Api.Models.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public bool IsCheckedOut { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public decimal TotalBasePrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDeleted { get; set; } = false;

        public List<CartItemDto>? CartItems { get; set; }
    }
}
