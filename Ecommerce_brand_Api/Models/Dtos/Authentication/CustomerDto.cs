namespace Ecommerce_brand_Api.Models.Dtos.Authentication
{
    public class CustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }  
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
