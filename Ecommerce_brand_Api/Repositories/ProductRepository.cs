namespace Ecommerce_brand_Api.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<Product?> GetByIdWithImagesAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductImagesPaths)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetAllWithImagesAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImagesPaths)
                .Include(p => p.ProductSizes)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }
    }
}
