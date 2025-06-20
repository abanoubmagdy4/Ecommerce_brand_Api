namespace Ecommerce_brand_Api.Models.Dtos
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        [MaxLength(70, ErrorMessage = "Name must not exceed 70 characters")]
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
    }
}
