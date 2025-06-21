namespace Ecommerce_brand_Api.Models.Dtos.Authentication
{
    public class AddressDto
    {

        [Required]
        [MaxLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public GovernrateShippingCostDto GovernrateShippingCostDto { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Apartment { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Building { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Floor { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}
