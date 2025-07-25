using Ecommerce_brand_Api.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        /// <summary>
        /// تعديل الكمية أو بيانات عنصر في الكارت
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemDto cartItemDto)
        {
            var result = await _cartItemService.UpdateCartItem(cartItemDto);
            return Ok(result);
        }

        /// <summary>
        /// حذف عنصر من الكارت
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var result = await _cartItemService.DeleteCartItemFromCart(cartItemId);
            return Ok(result);
        }

        // <summary>
        /// إضافة منتج إلى الكارت
        /// </summary>
        
        [HttpPost("add-single")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItemDto)
            {
            var result = await _cartItemService.AddCartItemToCurrentCart(cartItemDto);
            return Ok(result);
        }

        /// <summary>
        /// إضافة منتج إلى الكارت
        /// </summary>

        [HttpPost("add-multiple")]
        public async Task<IActionResult> AddToCartItems([FromBody] List<CartItemDto> cartItemDtos)
        {
            var result = await _cartItemService.AddCartItemsToCurrentCart(cartItemDtos);
            return Ok(result);
        }
    }
}