namespace Ecommerce_brand_Api.Models.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        //Total Price After Each Item's Discount => Not Including Any Other Special Discounts
        public List<CartItemDto>? CartItems { get; set; }
        public decimal TotalBasePrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
