namespace Ecommerce_brand_Api.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; } // to order
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal TotalBasePrice { get; set; }
        //public int DiscountId { get; set; } 
        //public Discount Discount { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation Property
        public ApplicationUser User { get; set; } = null!;
        public Discount Discount { get; set; }
        public Address Address { get; set; }

        /// <summary>
        /// Updates the total base price, total amount, and the last updated timestamp for the cart.
        /// </summary>
        /// <remarks>This method recalculates the total base price by summing the prices of all items in
        /// the cart, applies the discount to determine the total amount, and updates the timestamp to the current UTC
        /// time.</remarks>
        public void UpdateTotals()
        {
            TotalBasePrice = CartItems.Sum(item => item.TotalPriceForOneItemType);
            //TotalAmount = TotalBasePrice - Discount.DicountValue;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
