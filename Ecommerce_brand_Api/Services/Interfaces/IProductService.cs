using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IProductService: IBaseService<Product>
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task AddAsync(ProductDto dto);
        Task<bool> UpdateAsync(int id, ProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DecreaseStockAsync(int productId, int quantity);
        Task<bool> IncreaseStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
    }
}
