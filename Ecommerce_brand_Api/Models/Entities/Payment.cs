namespace Ecommerce_brand_Api.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public int PaidAmountCents { get; set; }
        public string Currency { get; set; }
        public bool IsRefunded { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsReturn { get; set; }
        public bool IsVoided { get; set; }
        public bool IsPaymentLocked { get; set; }
        public bool Pending { get; set; }
        public bool IsLive { get; set; }

        public string? Notes { get; set; }
        public string? ApiSource { get; set; }

        // Source Data
        public string? SourceType { get; set; }
        public string? SourceSubType { get; set; }
        public string? SourcePhoneNumber { get; set; }

        // Dates
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }

        // public ICollection<PaymentItem> Items { get; set; }
    }

}
