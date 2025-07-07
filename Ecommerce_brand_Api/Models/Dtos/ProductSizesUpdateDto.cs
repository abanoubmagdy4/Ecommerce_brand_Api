namespace Ecommerce_brand_Api.Models.Dtos
{
    public class ProductSizesUpdateDto
    {
        public int ProductId { get; set; }

        public List<ProductSizesDto> ProductSizes { get; set; } = new();
    }
}
