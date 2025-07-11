using Newtonsoft.Json;

namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class OrderSummaryDto
    {
        public int? OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public long? TransactionId { get; set; }

        public string? CustomerFullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? OrderAddressInfo { get; set; }

        public string? PaymentStatus { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PaymentMethod { get; set; }

        public decimal? ShippingCost { get; set; }
        public decimal? TotalOrderPrice { get; set; }
        public decimal? DiscountValue { get; set; }

        public string? OrderStatus { get; set; }
        public string? ShippingStatus { get; set; }

        public int? PaymobOrderId { get; set; }
        public string? PaymobOrderReference { get; set; }

        public bool? IsRefunded { get; set; }
        public bool? IsCanceled { get; set; }

        public int? MatchScore { get; set; }

        public List<OrderItemDTO> Items { get; set; } = new();
    }

}
