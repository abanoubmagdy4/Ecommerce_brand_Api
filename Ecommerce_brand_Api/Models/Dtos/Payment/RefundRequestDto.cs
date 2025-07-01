namespace Ecommerce_brand_Api.Models.Dtos.Payment
{
    public class RefundRequestDto
    {
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
