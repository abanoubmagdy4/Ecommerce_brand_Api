using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class PreviousOrderDto
    {
        [BindNever]
        public int OrderId { get; set; }

        [StringLength(20, ErrorMessage = "OrderNumber can't exceed 20 characters")]
        public string? OrderNumber { get; set; }//

        [BindNever]
        [Required]
        public DateTime CreatedAt { get; set; }//

        public DateTime? DeliveredAt { get; set; }//


        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingCost { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOrderPrice { get; set; }

        [Required]
        public string paymentMethod { get; set; }


        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public string ShippingStatus { get; set; }

        [Required]
        public List<string?> RefundStatuses { get; set; }


        [Required]
        public ICollection<PreviousOrderItemDto> OrderItems { get; set; }

        [Required]
        public string OrderAddressInfo { get; set; }
    }
}
