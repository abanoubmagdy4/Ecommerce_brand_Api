namespace Ecommerce_brand_Api.Models.Entities
{
    public class ProductRefund
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public decimal RefundedAmount { get; set; }
        public string Reason { get; set; } = string.Empty;

        public RefundStatus Status { get; set; } = RefundStatus.Pending;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }

        public string RequestedByUserId { get; set; } = null!;
        public ApplicationUser RequestedByUser { get; set; } = null!;

        public string? ApprovedByAdminId { get; set; }
    }
}
