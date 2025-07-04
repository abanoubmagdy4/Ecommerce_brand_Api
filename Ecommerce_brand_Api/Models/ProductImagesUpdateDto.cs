using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Models
{
    public class ProductImagesUpdateDto
    {
        public int ProductId { get; set; }

        public List<ProductImagesPathsDto> ProductImagesPaths { get; set; } = new();
    }
}
