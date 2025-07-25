namespace Ecommerce_brand_Api.Models.Dtos
{
    public class ContactUsDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
