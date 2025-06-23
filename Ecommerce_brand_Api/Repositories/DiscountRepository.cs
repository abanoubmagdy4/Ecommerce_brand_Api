namespace Ecommerce_brand_Api.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;

        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Discount?> GetActiveDiscountAsync()
        {
            return await _context.Discounts.FirstOrDefaultAsync();
        }
    }
}
