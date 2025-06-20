namespace Ecommerce_brand_Api.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }  // FK
        public IdentityUser User { get; set; } = null!;  // Navigation property

        public bool IsCheckedOut { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalBasePrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }

        public bool IsDeleted { get; set; } = false;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
