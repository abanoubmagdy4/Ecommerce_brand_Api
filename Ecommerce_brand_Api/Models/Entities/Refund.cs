namespace Ecommerce_brand_Api.Models.Entities
{
    public class Refund
    {
        public int Id { get; set; }  // 👈 المفتاح الأساسي
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Reason { get; set; } = string.Empty;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
