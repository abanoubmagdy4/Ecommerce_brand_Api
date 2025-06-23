namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IServiceUnitOfWork : IDisposable
    {
        IBaseService<T> GetBaseService<T>() where T : class;

        ICartService Carts { get; }
        ICategoryService Categories { get; }
        IFeedbackService Feedbacks { get; }
        IGovernrateShippingCostService GovernratesShippingCosts { get; }
        IOrderService Orders { get; }
        IProductService Products { get; }
        ITokenService Tokens { get; }
        IUserService Users { get; }

        Task<int> SaveChangesAsync();
    }
}
