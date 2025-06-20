namespace Ecommerce_brand_Api.Models.Entities
{
    public class Feedback
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int ProductId { get; set; }

        public int Rating { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;


    }
}
