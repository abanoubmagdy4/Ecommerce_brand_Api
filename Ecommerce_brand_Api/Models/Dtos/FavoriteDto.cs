namespace Ecommerce_brand_Api.Models.Dtos
{
    public class FavoriteDto
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
     

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
