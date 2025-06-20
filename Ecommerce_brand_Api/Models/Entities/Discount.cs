namespace Ecommerce_brand_Api.Models.Entities
{
    public class Discount
    {
        public int DicountId { get; set; }
        public decimal DicountValue { get; set; }

        public int orderId { get; set; }

        public Order order { get; set; }
    }
}
