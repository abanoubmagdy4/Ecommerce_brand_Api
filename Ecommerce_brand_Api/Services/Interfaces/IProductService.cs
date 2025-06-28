using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(PaginationParams pagination);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task AddAsync(ProductDto dto);
        Task<ServiceResult> AddProductSizeToProductAsync(List<ProductSizeDto> dtoList);
        Task<bool> UpdateAsync(ProductDto dto);
        Task<bool> UpdateProductSizes(List<ProductSizeDto> productsSizesDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DecreaseStockAsync(int productId, int quantity);
        Task<bool> IncreaseStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
    }
}
