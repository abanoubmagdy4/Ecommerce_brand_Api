using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateProduct([FromBody] FeedbackDto dto)
        {
           
            var result = await _feedbackService.RateProductAsync(dto);
            if (!result)
                return StatusCode(500, new { message = "Error while rating the product." });

            return Ok(new { message = "Rating submitted successfully." });
        }

        [HttpGet("average/{productId}")]
        public async Task<IActionResult> GetAverageRating(int productId)
        {
            var average = await _feedbackService.GetAverageRatingAsync(productId);
            return Ok(new { productId, averageRating = average });
        }
    }

}
