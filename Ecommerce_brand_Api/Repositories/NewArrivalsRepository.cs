
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

        public async Task<Product> GetNewArrivalByProductId(int productId)
        {
            return await _context.NewArrivals
                .Where(na => na.ProductId == productId)
                .Select(na => na.Product)
                .FirstOrDefaultAsync(); 
        }

        public async Task DeleteProductFromAsync(int productId)
        {
            var newArrivals = await _context.NewArrivals
                .Where(na => na.ProductId == productId)
                .ToListAsync();

            if (newArrivals.Any())
            {
                _context.NewArrivals.RemoveRange(newArrivals);
                await _context.SaveChangesAsync();
            }
        }

    }
}
