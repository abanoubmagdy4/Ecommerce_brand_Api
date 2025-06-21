using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_brand_Api.Models.Dtos
{
    public class ProductImagesPathsDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        [SwaggerSchema("Upload Image", Format = "binary")]
        public IFormFile? File { get; set; }
    }
}
