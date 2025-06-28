namespace Ecommerce_brand_Api.Models.Entities
{
    public class OtpCode
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpireAt { get; set; }
    }
}
