namespace Ecommerce_brand_Api.Models.Dtos.Authentication
{
    public class LoginDto
    {

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public bool IsRemember { get; set; } = false;
    }

}
