﻿namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IUnitofwork : IDisposable
    {
        IBaseRepository<T> GetBaseRepository<T>() where T : class;

        IOrderRepository GetOrderRepository();
        ICategoryRepository Categories { get; }
        IGovernrateShippingCostRepository GovernratesShippingCosts { get; }

        //IGovernrateShippingCostRepository GovernorateShippingCost { get; }
        IProductRepository Products { get; }
        ICartRepository Carts { get; }
        //ICartItemRepository CartItems { get; }
        IFeedbackRepository Feedbacks { get; }
        //IFavoriteRepository Favorites { get; }
        IDiscountRepository Discount { get; }
        IUserRepository User { get; }


        Task<int> SaveChangesAsync();
    }
}
