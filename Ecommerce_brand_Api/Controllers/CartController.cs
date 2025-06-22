using Ecommerce_brand_Api.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            this._cartService = cartService;
        }

        /// <summary>
        /// Retrieves the current user's cart.
        /// </summary>
        /// <remarks>This method fetches the cart associated with the current user. If no cart exists or
        /// the cart is empty,  a 404 Not Found response is returned. In the event of a server error, a 500 Internal
        /// Server Error  response is returned.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the cart data if found, or an appropriate error response.</returns> 
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiErrorResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<IActionResult> GetCurrentUserCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var cart = await _cartService.GetCurrentUserCartAsync();

                if (cart == null)
                {
                    return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "No Carts found."));
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse(StatusCodes.Status500InternalServerError, $"Server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Retrieves a cart by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the cart to retrieve. Must be a positive integer.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the operation: <list type="bullet"> <item>
        /// <description>A 200 OK response with a success message if the cart is found.</description> </item> <item>
        /// <description>A 404 Not Found response with an error message if no cart is found for the specified
        /// identifier.</description> </item> <item> <description>A 500 Internal Server Error response with an error
        /// message if a server error occurs.</description> </item> </list></returns>
        [HttpGet("{Id:int}")]
        [ProducesResponseType(typeof(ApiErrorResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<IActionResult> getCartById()
        {
            try
            {
                var curretnUserCart = await _cartService.GetCurrentUserCartAsync();

                if (curretnUserCart == null)
                {
                    return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, "No Cart found."));
                }

                return Ok(new ApiErrorResponse(StatusCodes.Status200OK, "Cart fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse(StatusCodes.Status500InternalServerError, $"Server error: {ex.Message}"));
            }

        }

        /// <summary>
        /// Creates a new shopping cart based on the provided cart data.
        /// </summary>
        /// <remarks>This method validates the provided cart data before attempting to create a new cart. 
        /// If the data is invalid, a 400 Bad Request response is returned.  If the cart is successfully created, a 201
        /// Created response is returned with a reference to the created cart. In case of an unexpected server error, a
        /// 500 Internal Server Error response is returned.</remarks>
        /// <param name="cartDto">The data transfer object containing the details of the cart to be created.  This parameter must not be null
        /// and must pass model validation.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation: <list type="bullet"> <item>
        /// <description> Returns a 201 Created response with the created cart's details if the operation is successful.
        /// </description> </item> <item> <description> Returns a 400 Bad Request response if the provided cart data is
        /// invalid. </description> </item> <item> <description> Returns a 500 Internal Server Error response if an
        /// unexpected error occurs on the server. </description> </item> </list></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiErrorResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<IActionResult> AddNewCart([FromBody] CartDto cartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Invalid order data"));
                }

                var createdCart = await _cartService.AddNewCartAsync(cartDto);

                return CreatedAtAction(
                    nameof(getCartById),
                    new { Id = createdCart.Id },
                    new ApiErrorResponse(
                        StatusCodes.Status201Created, "Cart created successfully"
                        )
                    );
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse(StatusCodes.Status500InternalServerError, $"Server error: {ex.Message}"));
            }

        }
    }
}