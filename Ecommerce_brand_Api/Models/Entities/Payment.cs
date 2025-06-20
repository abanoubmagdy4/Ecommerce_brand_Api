namespace Ecommerce_brand_Api.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }  // 👈 المفتاح الأساسي

        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Status { get; set; } = string.Empty;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
