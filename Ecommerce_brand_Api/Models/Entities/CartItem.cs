namespace Ecommerce_brand_Api.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }



        public decimal Discount { get; set; }
        public bool IsDeleted { get; set; } = false;
        public decimal TotalPrice { get; set; }  // = (UnitPrice * Quantity) - Discount

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;


    }
}
