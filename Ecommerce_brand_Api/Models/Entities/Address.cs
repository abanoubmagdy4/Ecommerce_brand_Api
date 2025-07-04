﻿namespace Ecommerce_brand_Api.Models.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [ForeignKey("GovernorateShippingCost")]
        public int GovernorateShippingCostId { get; set; }

        [Required]
        public GovernorateShippingCost GovernorateShippingCost { get; set; }

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

        public List<Order> Order { get; set; }
    }
}
