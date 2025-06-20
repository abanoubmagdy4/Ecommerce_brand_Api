namespace Ecommerce_brand_Api.Models.Dtos
{
    public class OrderItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "TotalPrice must be a positive number")]
        public decimal TotalPrice { get; set; }
    }
}
