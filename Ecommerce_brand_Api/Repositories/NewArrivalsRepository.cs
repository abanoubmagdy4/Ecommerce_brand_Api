
namespace Ecommerce_brand_Api.Repositories
{
    public class NewArrivalsRepository : BaseRepository<NewArrivals>, INewArrivalsRepository
    {
        public NewArrivalsRepository(AppDbContext context) : base(context)
        {
        }

        public Task<bool> IsProductNewArrivalAsync(int productId)
        {
            return _context.NewArrivals
                .AnyAsync(na => na.ProductId == productId && !na.Product.IsDeleted);
        }
    }
}
