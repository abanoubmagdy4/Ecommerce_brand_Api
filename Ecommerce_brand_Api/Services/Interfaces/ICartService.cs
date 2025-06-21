using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface ICartService
    {

        Task<IEnumerable<CartDto>> GetCartAsync();

        Task<CartDto?> GetCartByIdAsync(int Id);

        Task<CartDto> AddNewCartAsync(CartDto cart);

        Task<bool> UpdateCartAsync(int Id, CartDto cart);

        Task<bool> DeleteCartAsync(int Id);
    }
}
