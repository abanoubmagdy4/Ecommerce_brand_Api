namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface INewArrivalsRepository : IBaseRepository<NewArrivals>
    {
        //check if this product is already a new arrival
        Task<bool> IsProductNewArrivalAsync(int productId);

    }
}
