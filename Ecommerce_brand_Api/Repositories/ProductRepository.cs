using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public IQueryable<Product> GetAllDeletedProductsQueryable()
        {
            IQueryable<Product> query = _context.Set<Product>().AsQueryable()
                  .IgnoreQueryFilters()
                     .Include(p => p.ProductSizes)
                     .Include(p => p.ProductImagesPaths)
                      .Where(p => p.IsDeleted == true);

            return query;
        }
        public IQueryable<Product> GetAllProductsForAdminDashboardQueryable()
        {
            IQueryable<Product> query = _context.Set<Product>().AsQueryable()
                      .IgnoreQueryFilters()
                     .Include(p => p.ProductSizes)
                     .Include(p => p.ProductImagesPaths)
                     .Where(p => p.IsDeleted == false);

            return query;
        }
        public async Task<Product?> GetProductById(int id)
        {
            Product? product = _context.Products.IgnoreQueryFilters().FirstOrDefault(p => p.Id == id);
            return product;
        }

    }
}
