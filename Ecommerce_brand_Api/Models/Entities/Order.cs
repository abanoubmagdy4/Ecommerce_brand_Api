using Ecommerce_brand_Api.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_brand_Api.Models.Entities
{
    public class Order
    {
        [BindNever]
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string CustomerId { get; set; }
        public int ShippingAddressId { get; set; }
        public decimal? ShippingCost { get; set; }
        public int? DiscountId { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

        //NavigationProperty  
        public Payment Payment { get; set; }
        public Cancelation? Cancelation { get; set; }
        public Address ShippingAddress { get; set; }
        public ApplicationUser Customer { get; set; }
        public Discount? Discount { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
