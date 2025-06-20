namespace Ecommerce_brand_Api.Models.Entities
{
    public class PasswordResetCode
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(6)]
        public string Code { get; set; }

        [MaxLength(512)]
        public string? Token { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}
