using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Repositories;
using Ecommerce_brand_Api.Repositories.Interfaces;
using Ecommerce_brand_Api.Services.Interfaces;

namespace Ecommerce_brand_Api.Services
{
    public class FeedbackService : BaseService<Feedback>,IFeedbackService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public FeedbackService(IUnitofwork unitOfWork, IMapper mapper,ICurrentUserService currentUserService) : base(unitOfWork.GetBaseRepository<Feedback>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }


        public async Task<bool> RateProductAsync(FeedbackDto dto)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (userId == null)
                {
                   return false;    
                }
                var existing = await _unitOfWork.Feedbacks
                    .GetByUserAndProductAsync(userId, dto.ProductId);

                if (existing != null)
                {
                    existing.Rating = dto.Rating;
                    existing.CreatedAt = DateTime.UtcNow;
                    existing.UserId = userId;   
                    existing.ProductId = dto.ProductId; 
                }
                else
                {
                    var feedback = new Feedback()
                    {
                     ProductId = dto.ProductId, 
                    UserId = userId,
                   CreatedAt = DateTime.UtcNow,
                   Rating = dto.Rating, 
         
                    };
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
