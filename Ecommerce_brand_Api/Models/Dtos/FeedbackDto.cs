namespace Ecommerce_brand_Api.Models.Dtos
{
    public class FeedbackDto
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
       

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
