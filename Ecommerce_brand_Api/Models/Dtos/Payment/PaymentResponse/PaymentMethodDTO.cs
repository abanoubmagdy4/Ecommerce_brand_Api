namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class PaymentMethodDTO
    {
        public long Integration_Id { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Method_Type { get; set; }
        public string Currency { get; set; }
        public bool Live { get; set; }
        public bool Use_Cvc_With_Moto { get; set; }
    }
}
