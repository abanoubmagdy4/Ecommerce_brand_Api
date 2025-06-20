using Ecommerce_brand_Api.Helpers.Enums;

namespace Ecommerce_brand_Api.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }
        public decimal? ShippingCost { get; set; }
        public int? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }
        public Cancelation? Cancelation { get; set; }
    }
}
