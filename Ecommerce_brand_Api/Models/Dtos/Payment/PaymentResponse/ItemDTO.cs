namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class ItemDTO
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }

    }
}
