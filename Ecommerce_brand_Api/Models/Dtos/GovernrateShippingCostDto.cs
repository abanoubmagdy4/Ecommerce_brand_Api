namespace Ecommerce_brand_Api.Models.Dtos
{
    public class GovernrateShippingCostDto
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        [MaxLength(70, ErrorMessage = "Name must not exceed 70 characters")]
        public string Name { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Shipping cost must be a positive value")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ShippingCost { get; set; }

    }
}
