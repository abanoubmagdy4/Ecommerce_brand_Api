namespace Ecommerce_brand_Api.Models.Entities
{
    public class NewArrivals
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
