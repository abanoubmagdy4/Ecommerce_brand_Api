namespace Ecommerce_brand_Api.Repositories
{
    public class GovernrateShippingCostRepository : BaseRepository<GovernorateShippingCost>, IGovernrateShippingCostRepository
    {
        public GovernrateShippingCostRepository(AppDbContext context) : base(context)
        {
        }
    }
}
