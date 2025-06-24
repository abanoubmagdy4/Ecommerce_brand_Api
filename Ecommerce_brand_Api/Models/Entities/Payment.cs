namespace Ecommerce_brand_Api.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Currency { get; set; }
        public bool IsPaymentLocked { get; set; }
        public bool IsReturn { get; set; }
        public bool IsCancel { get; set; }
        public bool IsReturned { get; set; }
        public bool IsCanceled { get; set; }
        public int PaidAmountCents { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string SourceType { get; set; }
        public string SourcePhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign Keys
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }

        public List<PaymentItem> Items { get; set; }
    }

}
