using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernorateShippingCostController : ControllerBase
    {
        private readonly IGovernrateShippingCostService _governorateService;

        public GovernorateShippingCostController(IGovernrateShippingCostService governorateService)
        {
            _governorateService = governorateService;
        }

        /// <summary>
        /// Get all governorate shipping costs
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GovernrateShippingCostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _governorateService.GetAllAsync();
            if (result == null || !result.Any())
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "No governorates found."));
            return Ok(result);
        }

        /// <summary>
        /// Get shipping cost by governorate ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GovernrateShippingCostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _governorateService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "Governorate not found."));
            return Ok(result);
        }

        /// <summary>
        /// Add a new governorate shipping cost
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] GovernrateShippingCostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Invalid data."));

            await _governorateService.AddAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Update a governorate shipping cost by ID
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] GovernrateShippingCostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Invalid data."));

            var result = await _governorateService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "Governorate not found."));

            return NoContent();
        }

        /// <summary>
        /// Delete a governorate shipping cost by ID
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _governorateService.DeleteAsync(id);
            if (!result)
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "Governorate not found."));
            return NoContent();
        }
    }
}
