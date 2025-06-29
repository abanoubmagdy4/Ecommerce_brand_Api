namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IGovernrateShippingCostRepository : IBaseRepository<GovernorateShippingCost>
    {
        Task<GovernorateShippingCost?> GetByNameAsync(string name);
    }
}
