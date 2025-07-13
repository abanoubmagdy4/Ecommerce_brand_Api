using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class OrderDTO
    {
        [BindNever]
        public int OrderId { get; set; }

        [StringLength(20, ErrorMessage = "OrderNumber can't exceed 20 characters")]
        public string? OrderNumber { get; set; }

        [BindNever]
        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public string CustomerId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingCost { get; set; }




        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOrderPrice { get; set; }

        [Required]
        [EnumDataType(typeof(PaymentMethods), ErrorMessage = "Invalid PaymentMethods Status")]
        public PaymentMethods paymentMethod { get; set; }


        [Required]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid Order Status")]
        public OrderStatus OrderStatus { get; set; }

        [Required]
        public ICollection<OrderItemDTO> OrderItems { get; set; }

        [Required]
        public CustomerDto CustomerInfo { get; set; }
        [Required]
        public AddressDto AddressInfo { get; set; }
    }
}
