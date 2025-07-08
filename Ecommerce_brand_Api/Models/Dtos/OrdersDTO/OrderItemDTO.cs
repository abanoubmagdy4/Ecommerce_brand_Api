using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class OrderItemDTO
    {
        [BindNever]
        public int OrderItemId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]

        public int ProductSizeId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "TotalPrice must be a positive number")]
        public decimal TotalPrice { get; set; }
    }
}
