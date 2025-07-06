namespace Ecommerce_brand_Api.Models.Dtos.Payment
{
    public class ProductRefundDto
    {
        public int OrderItemId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
