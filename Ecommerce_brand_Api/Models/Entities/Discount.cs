namespace Ecommerce_brand_Api.Models.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public decimal DicountValue { get; set; } = 0;

        public int orderId { get; set; }
        public Order order { get; set; }
    }
}
