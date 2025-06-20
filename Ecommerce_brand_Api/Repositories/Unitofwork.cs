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

        public IOrderRepository GetOrderRepository()
        {
            var service = _ServiceProvider.GetService<IOrderRepository>();

            if (service == null)
            {

                throw new InvalidOperationException("No Service Resistered for IOrderRepository");
            }
            return service;
        }

        public ICategoryRepository Categories => _ServiceProvider.GetService<ICategoryRepository>()?? throw new InvalidOperationException("CategoryRepository not found");

        public Task<int> SaveChangesAsync() => _Context.SaveChangesAsync();

        public void Dispose()
        {
            _Context.Dispose();
        }
    }
}
