namespace Ecommerce_brand_Api.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal TotalBasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }

        public IdentityUser User { get; set; } = null!;

        public void UpdateTotals()
        {
            TotalBasePrice = CartItems.Sum(item => item.TotalPriceForOneItemType);
            TotalAmount = TotalBasePrice - Discount;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
