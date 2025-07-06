using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Models;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IProductService : IBaseService<Product>
    {
        Task<PaginatedResult<ProductDtoResponse>> GetPaginatedProductsAsync(ProductFilterParams pagination);
        Task<IEnumerable<ProductDtoResponse>> GetAllAsync();
        Task<ProductDtoResponse?> GetByIdAsync(int id);
        Task<ServiceResult> AddProductAsync(ProductDtoRequest dto);
        Task<ServiceResult> AddProductSizeToProductAsync(List<ProductSizesDto> dtoList);
        Task<bool> UpdateBasicInfoAsync(ProductBaseUpdateDto dto);
        Task<bool> ReplaceImageByIdAsync(int imageId, IFormFile newImageFile);
        Task<bool> UpdateProductSizesAsync(ProductSizesUpdateDto dto);
        Task<bool> UpdateProductSizes(List<ProductSizesDto> productsSizesDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DecreaseStockAsync(int productId, int quantity);
        Task<bool> IncreaseStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductDtoResponse>> GetByCategoryAsync(int categoryId);
        Task<ProductDtoResponse> AddToNewArrivals(int Id);
    }
}
