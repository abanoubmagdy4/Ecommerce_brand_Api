namespace Ecommerce_brand_Api.Models.Entities
{
    public class PaymentItem
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AmountCents { get; set; }
        public int Quantity { get; set; }
    }
}
