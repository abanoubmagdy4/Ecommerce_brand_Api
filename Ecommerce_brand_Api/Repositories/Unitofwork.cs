

namespace Ecommerce_brand_Api.Repositories
{
    public class Unitofwork : IUnitofwork
    {
        private readonly AppDbContext _Context;
        private readonly IServiceProvider _ServiceProvider;

        public Unitofwork(AppDbContext context, IServiceProvider serviceProvider)
        {

            _ServiceProvider = serviceProvider;
            _Context = context;
        }

        public IBaseRepository<T> GetBaseRepository<T>() where T : class
        {
            var service = _ServiceProvider.GetService<IBaseRepository<T>>();
            if (service == null)
            {

                throw new InvalidOperationException("No Service Resistered for IBaseRepository");
            }
            return service;
        }

        /// <summary>
        /// Retrieves an instance of the <see cref="IOrderRepository"/> service from the service provider.
        /// </summary>
        /// <remarks>This method resolves the <see cref="IOrderRepository"/> implementation registered in
        /// the service provider. If no implementation is registered, an exception is thrown.</remarks>
        /// <returns>An instance of the <see cref="IOrderRepository"/> service.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no service is registered for <see cref="IOrderRepository"/>.</exception> 
        public IOrderRepository GetOrderRepository()
        {
            var service = _ServiceProvider.GetService<IOrderRepository>();

            if (service == null)
            {
                throw new InvalidOperationException("No Service Resistered for IOrderRepository");
            }
            return service;
        }

        /// <summary>
        /// Gets the repository for managing category-related data and operations.
        /// </summary> 
        public ICategoryRepository Categories => _ServiceProvider.GetService<ICategoryRepository>() ?? throw new InvalidOperationException("CategoryRepository not found");

        /// <summary>
        /// Gets the repository for managing governrate-specific shipping costs.
        /// </summary> 
        public IGovernrateShippingCostRepository GovernratesShippingCosts =>
            _ServiceProvider.GetService<IGovernrateShippingCostRepository>()
            ?? throw new InvalidOperationException("No service registered for IGovernrateShippingCostRepository");

        /// <summary>
        /// Gets the repository for managing product data.
        /// </summary>
        public IProductRepository Products =>
         _ServiceProvider.GetService<IProductRepository>()
         ?? throw new InvalidOperationException("No service registered for IProductRepository");

        public IProductSizesRepository ProductsSizes =>
        _ServiceProvider.GetService<IProductSizesRepository>()
        ?? throw new InvalidOperationException("No service registered for IProductRepository");


        public ICartRepository Carts => _ServiceProvider.GetService<ICartRepository>()
            ?? throw new InvalidOperationException("No service registered for CartRepository");

        public ICartItemRepository CartItem => _ServiceProvider.GetService<ICartItemRepository>()
            ?? throw new InvalidOperationException("No service registered for CartItemRepository");


        public IFeedbackRepository Feedbacks =>
             _ServiceProvider.GetRequiredService<IFeedbackRepository>()
            ?? throw new InvalidOperationException("No service registered for FeedbackRepository");

        public INewArrivalsRepository NewArrivals =>
             _ServiceProvider.GetRequiredService<INewArrivalsRepository>()
            ?? throw new InvalidOperationException("No service registered for NewArrivalsRepository");

        public Task<int> SaveChangesAsync() => _Context.SaveChangesAsync();

        public void Dispose()
        {
            _Context.Dispose();
        }
    }
}




