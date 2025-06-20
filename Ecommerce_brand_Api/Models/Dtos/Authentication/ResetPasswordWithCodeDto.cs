namespace Ecommerce_brand_Api.Models.Dtos.Authentication
{
    public class ResetPasswordWithCodeDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
