namespace Ecommerce_brand_Api.Models.Dtos.Authentication
{
    public class PasswordResetDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
