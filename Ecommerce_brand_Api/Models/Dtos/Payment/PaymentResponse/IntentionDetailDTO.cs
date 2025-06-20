namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class IntentionDetailDTO
    {
        public decimal Amount { get; set; }
        public List<ItemDTO> Items { get; set; }
        public string Currency { get; set; }
        public BillingDataDTO Billing_Data { get; set; }
    }
}
