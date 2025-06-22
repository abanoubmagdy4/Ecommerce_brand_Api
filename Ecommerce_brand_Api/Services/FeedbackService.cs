using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitofwork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> RateProductAsync(FeedbackDto dto)
        {
            try
            {
                var existing = await _unitOfWork.Feedbacks
                    .GetByUserAndProductAsync(dto.UserId, dto.ProductId);

                if (existing != null)
                {
                    existing.Rating = dto.Rating;
                    existing.CreatedAt = DateTime.UtcNow;
                }
                else
                {
                    var feedback = _mapper.Map<Feedback>(dto);
                    await _unitOfWork.Feedbacks.AddAsync(feedback);
                }

                
                var avg = await _unitOfWork.Feedbacks.GetAverageRatingAsync(dto.ProductId);

                
                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                if (product != null)
                {
                    product.AverageRating = avg;
                }

                
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _unitOfWork.Feedbacks.GetAverageRatingAsync(productId);
        }
    }

}
