using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Models;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IProductService : IBaseService<Product>
    {
        Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(ProductFilterParams pagination);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task AddAsync(ProductDto dto);
        Task<ServiceResult> AddProductSizeToProductAsync(List<ProductSizesDto> dtoList);
        Task<bool> UpdateBasicInfoAsync(ProductBaseUpdateDto dto);
        Task<bool> UpdateProductImagesAsync(ProductImagesUpdateDto dto);
        Task<bool> UpdateProductSizesAsync(ProductSizesUpdateDto dto);


        Task<bool> UpdateAsync(ProductDto dto);
        Task<bool> UpdateProductSizes(List<ProductSizesDto> productsSizesDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DecreaseStockAsync(int productId, int quantity);
        Task<bool> IncreaseStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);

        Task<ProductDto> AddToNewArrivals(int Id);
    }
}
