
namespace Ecommerce_brand_Api.Repositories
{
    public class CartRepository : BaseRepository<Cart> , ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID", nameof(userId));

            return await _context.Carts
                .Include(c => c.CartItems) 
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }


    }
}
