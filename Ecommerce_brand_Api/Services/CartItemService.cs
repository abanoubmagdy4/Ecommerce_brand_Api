using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUserService;
        public CartItemService(IUnitofwork unitofwork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitofwork = unitofwork;
            this.mapper = mapper;
            this.currentUserService = currentUserService;
        }

        public async Task<CartItemDto> AddCartItemToCart(CartItemDto cartItemDto)
        {
            var cartRepo = _unitofwork.GetBaseRepository<Cart>();
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.UserId == currentUserService.UserId,
                include: q => q.Include(c => c.CartItems)
            );

            if (cart == null)
                throw new Exception("Cart not found");

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == cartItemDto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDto.Quantity;
                await cartItemRepo.UpdateAsync(existingItem);
                await _unitofwork.SaveChangesAsync();
                return mapper.Map<CartItemDto>(existingItem); // ✅ عملنا المابنج هنا
            }
            else
            {
                var cartItem = mapper.Map<CartItem>(cartItemDto);
                cartItem.CartId = cart.Id;
                await cartItemRepo.AddAsync(cartItem);
                await _unitofwork.SaveChangesAsync();
                return mapper.Map<CartItemDto>(cartItem); // ✅ هنا كمان
            }
        }


        public async Task<CartItemDto> UpdateCartItem(CartItemDto cartItemDto)
        {
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();

            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                ci => ci.Id == cartItemDto.Id && ci.Cart.UserId == currentUserService.UserId,
                include: q => q.Include(ci => ci.Cart)
            );

            if (cartItem == null)
                throw new Exception("Cart item not found");

            cartItem.Quantity = cartItemDto.Quantity;
            cartItem.TotalPriceForOneItemType = cartItem.UnitPrice * cartItem.Quantity;
            await cartItemRepo.UpdateAsync(cartItem);
            await _unitofwork.SaveChangesAsync();

            return mapper.Map<CartItemDto>(cartItem);
        }

        public async Task<CartItemDto> DeleteCartItemFromCart(int cartItemId)
        {
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();

            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                ci => ci.Id == cartItemId && ci.Cart.UserId == currentUserService.UserId,
                include: q => q.Include(ci => ci.Cart)
            );

            if (cartItem == null)
                throw new Exception("Cart item not found");

            await cartItemRepo.DeleteAsync(cartItem.Id);
            await _unitofwork.SaveChangesAsync();

            return mapper.Map<CartItemDto>(cartItem);
        }
    }


}
