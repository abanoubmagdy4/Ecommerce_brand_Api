using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Models
{
    public class ProductSizesUpdateDto
    {
        public int ProductId { get; set; }

        public List<ProductSizesDto> ProductSizes { get; set; } = new();
    }
}
