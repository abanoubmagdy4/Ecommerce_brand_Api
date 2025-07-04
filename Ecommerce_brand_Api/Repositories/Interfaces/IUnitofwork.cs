namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IUnitofwork : IDisposable
    {
        IBaseRepository<T> GetBaseRepository<T>() where T : class;

        IOrderRepository GetOrderRepository();
        ICategoryRepository Categories { get; }
        IGovernrateShippingCostRepository GovernratesShippingCosts { get; }

        //IGovernrateShippingCostRepository GovernorateShippingCost { get; }
        IProductRepository Products { get; }
        IProductSizesRepository ProductsSizes { get; }
        ICartRepository Carts { get; }
        //ICartItemRepository CartItems { get; }
        IFeedbackRepository Feedbacks { get; }
        //IFavoriteRepository Favorites { get; }

        INewArrivalsRepository NewArrivals { get; }

        IDiscountRepository Discount { get; }
        IUserRepository User { get; }
        IPaymentRepository Payment { get; }
        IRefundRequestRepository RefundRequest {get; }



        Task<int> SaveChangesAsync();
    }
}
