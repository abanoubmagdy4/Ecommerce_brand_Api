namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IUnitofwork : IDisposable
    {
        public IBaseRepository<T> GetBaseRepository<T>() where T :class;
        public IOrderRepository GetOrderRepository();
        ICategoryRepository Categories { get; }
        //IProductRepository Products { get; }
        //ICartRepository Carts { get; }
        //ICartItemRepository CartItems { get; }
        //IFeedbackRepository Feedbacks { get; }
        //IFavoriteRepository Favorites { get; }

        Task<int> SaveChangesAsync();

    }
}
