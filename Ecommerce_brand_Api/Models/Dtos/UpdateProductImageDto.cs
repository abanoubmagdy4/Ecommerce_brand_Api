namespace Ecommerce_brand_Api.Models.Dtos
{
    public class UpdateProductImageDto
    {
        public int ImageId { get; set; }

        public IFormFile ImageFile { get; set; } 
    }
}
