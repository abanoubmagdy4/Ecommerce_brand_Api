namespace Ecommerce_brand_Api.Models.Entities
{
    public class OrderRefund
    {
        public int Id { get; set; }

        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        public decimal AmountCents { get; set; }
        public string Reason { get; set; } = string.Empty;
        public RefundStatus Status { get; set; } = RefundStatus.Pending;

        public string RequestedByUserId { get; set; }
        public ApplicationUser RequestedByUser { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedByAdminId { get; set; }
    }

  


}

