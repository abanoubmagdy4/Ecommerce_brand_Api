namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class PaymentKeyDTO
    {
        public long Integration { get; set; }
        public string Key { get; set; }
        public string Gateway_Type { get; set; }
        public object Iframe_Id { get; set; }
        public long Order_Id { get; set; }
        public string Redirection_Url { get; set; }
        public bool Save_Card { get; set; }
    }
}
