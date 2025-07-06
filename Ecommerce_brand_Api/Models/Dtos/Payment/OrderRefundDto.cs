namespace Ecommerce_brand_Api.Models.Dtos.Payment
{
    public class OrderRefundDto
    {
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
