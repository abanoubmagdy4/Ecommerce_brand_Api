using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class PreviousOrderItemDto
    {
        [BindNever]
        public int OrderItemId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

         public string ImagePath { get; set; } = string.Empty;

        [Required]
        public string Size { get; set; }

        public float width { get; set; }
        public float Height { get; set; }

        [Required]
        public string? RefundStatus { get; set; }    

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "TotalPrice must be a positive number")]
        public decimal TotalPrice { get; set; }


    }
}
