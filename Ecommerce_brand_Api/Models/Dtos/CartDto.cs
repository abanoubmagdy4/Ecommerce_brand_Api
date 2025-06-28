using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Dtos
{
    public class CartDto
    {
        [BindNever]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public decimal TotalBasePrice { get; set; }
        public decimal Threshold { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemDto>? CartItems { get; set; }
    }
}

