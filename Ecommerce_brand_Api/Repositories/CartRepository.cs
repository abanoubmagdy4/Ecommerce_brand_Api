
namespace Ecommerce_brand_Api.Repositories
{
    public class CartRepository : BaseRepository<Cart> , ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
            
        }


    }
}
