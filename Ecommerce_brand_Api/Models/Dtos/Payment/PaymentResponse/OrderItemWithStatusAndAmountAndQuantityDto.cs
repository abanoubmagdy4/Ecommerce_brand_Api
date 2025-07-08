namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class OrderItemWithStatusAndAmountAndQuantityDto
    {
        public int OrderItemId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity {  get; set; }  
        public DateTime CreatedAt {  get; set; }    
    }
}
