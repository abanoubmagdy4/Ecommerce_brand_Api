namespace Ecommerce_brand_Api.Models.Dtos.OrdersDTO
{
    public class UpdateShippingStatusDto
    {
        public int OrderId { get; set; }
        public ShippingStatus NewShippingStatus { get; set; }
    }
}
