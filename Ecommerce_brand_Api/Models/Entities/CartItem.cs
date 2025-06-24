namespace Ecommerce_brand_Api.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductSizes))]
        public int ProductSizeId { get; set; }
        public ProductSizes ProductSize { get; set; } = null!;


        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPriceForOneItemType { get; set; }

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
