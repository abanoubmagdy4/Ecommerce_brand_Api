namespace Ecommerce_brand_Api.Models.Entities
{
    public class Cancelation
    {
        public int Id { get; set; } 

        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CancelledAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
