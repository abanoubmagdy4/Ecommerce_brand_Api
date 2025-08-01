﻿using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.BackgroundServices;
using Ecommerce_brand_Api.Models;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;
using Newtonsoft.Json;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly INewArrivalsService _newArrivalsService;

        public ProductsController(IProductService productService, INewArrivalsService newArrivalsService )
        {
            _productService = productService;
            _newArrivalsService = newArrivalsService;

        }


        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var products = await _productService.GetAllAsync();
        //    return Ok(products);
        //}


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDtoRequest productDtoRequest)
        {
            var result = await _productService.AddProductAsync(productDtoRequest);

            if (result.Success)
            { 
                return Ok(new
                {
                    success = true,
                    message = result.SuccessMessage
                });
            }
            else { 
            return BadRequest(new
            {
                success = false,
                error = result.ErrorMessage
            });
            }
        }


        [HttpPost]
        [Route("AddProductSizeToProduct")]
        public async Task<IActionResult> AddProductSizeToProduct(List<ProductSizesDto> dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var serviceResult = await _productService.AddProductSizeToProductAsync(dto);
            if (!serviceResult.Success)
                return BadRequest(new { message = serviceResult.ErrorMessage });
            return Ok(new { message = "Product Sizes were created successfully." });
        }




        [HttpPut("basic-update")]
        public async Task<IActionResult> UpdateProductBasicData([FromBody] ProductBaseUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _productService.UpdateBasicInfoAsync(dto);
            if (!updated)
                return NotFound(new { message = "Product not found." });

            return Ok(new { message = "Product updated successfully." });
        }

        [HttpPut("update-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateSingleImage(UpdateProductImageDto updateProductImageDto)
        {
            if (updateProductImageDto.ImageFile == null || updateProductImageDto.ImageFile.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var updated = await _productService.ReplaceImageByIdAsync(updateProductImageDto.ImageId, updateProductImageDto.ImageFile);

            return updated
                ? Ok(new { message = "Image updated successfully." })
                : NotFound(new { message = "Image not found." });
        }



        [HttpPut]
        [Route("UpdateProductSizes")]
        public async Task<IActionResult> UpdateProductSizes(List<ProductSizesDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateProductSizes(dtoList);

            if (result == null || result.Count == 0)
                return NotFound(new { message = "No product sizes were updated." });

            return Ok(new
            {
                message = "Product sizes updated successfully.",
                data = result
            });
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            return deleted
                ? Ok(new { message = "Product deleted successfully." })
                : NotFound();
        }

        [HttpPut("RestoreProduct/{id}")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            var deleted = await _productService.RestoreProduct(id);
            return deleted
                ? Ok(new { message = "Product Restored successfully." })
                : BadRequest();
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


        [HttpPost("decrease-stock")]
        public async Task<IActionResult> DecreaseStock(int productSizeId, int quantity)
        {
            var result = await _productService.DecreaseStockAsync(productSizeId, quantity);
            if (!result)
                return NotFound("Product size not found or insufficient stock.");

            return Ok("Stock decreased successfully.");
        }

        [HttpPost("increase-stock")]
        public async Task<IActionResult> IncreaseStock(int productSizeId, int quantity)
        {
            var result = await _productService.IncreaseStockAsync(productSizeId, quantity);
            if (!result)
                return NotFound("Product size not found.");

            return Ok("Stock increased successfully.");
        }
        [HttpGet("Custpaginated")]
        public async Task<IActionResult> GetPaginatedCustomerProducts([FromQuery] ProductFilterParams filter)
        {
            try
            {
                var result = await _productService.GetPaginatedProductsForCustomerAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching paginated products.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("AdDashpaginated")]
        public async Task<IActionResult> GetPaginatedAdminDashboardProducts([FromQuery] ProductFilterParams filter)
        {
            try
            {
                var result = await _productService.GetPaginatedProductsForAdminDashboardAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching paginated products.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("paginatedDeleted")]
        public async Task<IActionResult> GetPaginatedDeletedProducts([FromQuery] ProductFilterParams filter)
        {
            try
            {
                var result = await _productService.GetPaginatedDeletedProductsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching paginated products.",
                    error = ex.Message
                });
            }
        }
        [HttpPost("add-to-new-arrivals/{productId}")]
        public async Task<IActionResult> AddToNewArrivalsAsync(int productId)
        {
            try
            {
                await _newArrivalsService.AddNewArrivalAsync(productId);
                return Ok(new { message = "Product added to new arrivals successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while adding the product to new arrivals.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("new-arrivals")]
        public async Task<IActionResult> GetNewArrivals([FromQuery] PaginationParams pagination)
        {
            try
            {
                var newArrivals = await _newArrivalsService.GetNewArrivalsAsync(pagination);
                return Ok(newArrivals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching new arrivals.",
                    error = ex.Message
                });
            }
        }


        [HttpDelete("delete-new-arrival/{productId}")]
        public async Task<IActionResult> DeleteNewArrival(int productId)
        {
            try
            {
                var deleted = await _newArrivalsService.DeleteNewArrival(productId);
                return deleted
                               ? Ok(new { message = "New Arrival deleted successfully." })
                               : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error while deleting new arrival.", details = ex.Message });
            }
        }

      
    }
}
