namespace Ecommerce_brand_Api.Models.Entities
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;              // FK to Identity user

        public int ProductId { get; set; }                       // FK to Product
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;


    }
}
