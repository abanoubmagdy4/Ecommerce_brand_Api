namespace Ecommerce_brand_Api.Repositories
{
    public class ProductsSizesRepository : BaseRepository<ProductSizes>, IProductSizesRepository
    {
        public ProductsSizesRepository(AppDbContext context) : base(context)
        {

        }
    }
}
