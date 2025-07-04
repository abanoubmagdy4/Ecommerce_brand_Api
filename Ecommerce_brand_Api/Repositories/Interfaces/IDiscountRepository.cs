namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<Discount?> GetActiveDiscountAsync();
    }
}
