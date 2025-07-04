namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
    }
}
