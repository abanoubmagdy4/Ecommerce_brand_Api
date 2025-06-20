using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //private readonly IUserService _userService;
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            if (result == null)
            {
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "No categories found."));
            }

            return Ok(result);
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "No categories found."));
            }

            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _categoryService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _categoryService.UpdateAsync(id, dto);
            if (!result)
            {
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "Category not found."));
            }
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "Category not found."));
            }
            return NoContent();
        }
    }
}
