namespace Ecommerce_brand_Api.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public int PaymobOrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string CustomerId { get; set; }
        public int ShippingAddressId { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal DiscountValue { get; set; } = 0;
        //Customer Information For every single order (He can change it later but still saved as info for specific order)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        //Address info for single order
        public string OrderAddressInfo { get; set; }
        public PaymentMethods paymentMethod { get; set; }
        //NavigationProperty  
        public Payment Payment { get; set; }
        public Cancelation? Cancelation { get; set; }
        public Address ShippingAddress { get; set; }
        public ApplicationUser Customer { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
