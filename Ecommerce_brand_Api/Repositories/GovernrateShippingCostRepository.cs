namespace Ecommerce_brand_Api.Repositories
{
    public class GovernrateShippingCostRepository : BaseRepository<GovernorateShippingCost>, IGovernrateShippingCostRepository
    {
        private readonly AppDbContext context;

        public GovernrateShippingCostRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<GovernorateShippingCost?> GetByNameAsync(string name)
        {
            return await context.GovernratesShippingCosts.FirstOrDefaultAsync(g => g.Name == name);
        }

    }
}
