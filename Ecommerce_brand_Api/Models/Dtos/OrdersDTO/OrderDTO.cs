using Ecommerce_brand_Api.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class OrderDTO
    {
        [BindNever]
        public int OrderId { get; set; }

        [StringLength(20, ErrorMessage = "OrderNumber can't exceed 20 characters")]
        public string OrderNumber { get; set; }

        [BindNever]
        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        [Required]
        public int ShippingAddressId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingCost { get; set; }

        public int? DiscountId { get; set; }

        [Range(0, 100, ErrorMessage = "Discount Percentage must be between 0% and 100%")]
        public decimal DiscountValue { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOrderPrice { get; set; }

        [Required]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid Order Status")]
        public OrderStatus OrderStatus { get; set; }

        [Required]
        public ICollection<OrderItemDTO> OrderItems { get; set; }
    }
}
