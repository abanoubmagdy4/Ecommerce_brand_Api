using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IFeedbackService : IBaseService<Feedback>
    {
        Task<bool> RateProductAsync(FeedbackDto dto);
        Task<double> GetAverageRatingAsync(int productId);
    }
}
