using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productService.AddAsync(dto);
            return Ok(new { message = "Product created successfully." });
        }


        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _productService.UpdateAsync(id, dto);
            return updated
                ? Ok(new { message = "Product updated successfully." })
                : NotFound();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            return deleted
                ? Ok(new { message = "Product deleted successfully." })
                : NotFound();
        }


        //[HttpGet("ByCategory/{categoryId}")]
        //public async Task<IActionResult> GetByCategory(int categoryId)
        //{
        //    try
        //    {
        //        var products = await _productService.GetByCategoryAsync(categoryId);
        //        if (products == null || !products.Any())
        //            return NotFound(new { message = "No products found in this category." });

        //        return Ok(products);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error while retrieving products by category.", details = ex.Message });
        //    }
        //}


    }
}
