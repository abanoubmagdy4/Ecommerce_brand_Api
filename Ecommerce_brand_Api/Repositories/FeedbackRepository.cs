
namespace Ecommerce_brand_Api.Repositories
{
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {

        public FeedbackRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<Feedback?> GetByUserAndProductAsync(string userId, int productId)
        {
            return await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _context.Feedbacks
                .Where(f => f.ProductId == productId)
                .AverageAsync(f => (double?)f.Rating) ?? 0.0;
        }
    }
}
