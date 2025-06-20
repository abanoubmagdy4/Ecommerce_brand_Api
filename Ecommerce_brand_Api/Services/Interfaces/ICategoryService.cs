using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>>GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task AddAsync(CategoryDTO dto);
        Task<bool> UpdateAsync(int id, CategoryDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
