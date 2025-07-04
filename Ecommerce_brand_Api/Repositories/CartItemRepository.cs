namespace Ecommerce_brand_Api.Repositories
{
    public class CartItemRepository : BaseRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
