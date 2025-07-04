namespace Ecommerce_brand_Api.Models.Dtos.Authentication
{
    public class AddressDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        public int GovernrateShippingCostId { get; set; }

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
    }
}
