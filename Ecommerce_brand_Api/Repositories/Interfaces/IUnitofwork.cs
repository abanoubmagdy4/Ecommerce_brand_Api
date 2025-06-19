namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IUnitofwork : IDisposable
    {
        public IBaseRepository<T> GetBaseRepository<T>() where T :class;
        public IOrderRepository GetOrderRepository();

    }
}
