namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IFeedbackRepository : IBaseRepository<Feedback>
    {
        Task<Feedback?> GetByUserAndProductAsync(string userId, int productId);
        Task<double> GetAverageRatingAsync(int productId);
    }

}
