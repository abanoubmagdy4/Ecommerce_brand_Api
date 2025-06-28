using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductSizesRepository _productSizesRepository;
        private readonly ICartService _cartService;
        private readonly AppDbContext _db;
        public CartItemService(IUnitofwork unitofwork, IMapper mapper, ICurrentUserService currentUserService, IProductSizesRepository productSizesRepository, ICartService cartService, AppDbContext db)
        {
            _unitofwork = unitofwork;
            this.mapper = mapper;
            this._currentUserService = currentUserService;
            this._productSizesRepository = productSizesRepository;
            this._cartService = cartService;
            this._db = db;
        }

        public async Task<CartItemDto> AddCartItemToCurrentCart(CartItemDto cartItemDto)
        {
            var cartRepo = _unitofwork.GetBaseRepository<Cart>();
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();
            var userId = _currentUserService.UserId;


            // ✅ جلب ProductSize + المنتج
            var productSize = await _productSizesRepository.GetFirstOrDefaultAsync(
                ps => ps.Id == cartItemDto.ProductSizeId,
                include: q => q.Include(ps => ps.Product)
            );

            if (productSize == null || productSize.Product == null)
                throw new Exception("Product size or linked product not found");

            if (productSize.StockQuantity < cartItemDto.Quantity)
                throw new Exception("Insufficient product size quantity");

            var unitPrice = productSize.Product.Price;
            var itemTotal = unitPrice * cartItemDto.Quantity;

            // ✅ جلب أول خصم
            var discount = await _db.Discounts.FirstOrDefaultAsync();
            var discountValue = discount.DicountValue;

            var cart = await _cartService.GetCurrentUserCartAsync();

            if (cart == null)
            {
                var NewCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    TotalBasePrice = itemTotal,
                    TotalAmount = itemTotal - (itemTotal * discountValue),
                    CartItems = new List<CartItem>()
                };

                await cartRepo.AddAsync(NewCart);
                await _unitofwork.SaveChangesAsync();
            }

            var cartWithItems = await cartRepo.GetFirstOrDefaultAsync(
                c => c.Id == cart.Id,
                include: q => q.Include(c => c.CartItems)
            );

            var existingItem = cartWithItems.CartItems.FirstOrDefault(ci =>
                ci.ProductId == cartItemDto.ProductId &&
                ci.ProductSizeId == cartItemDto.ProductSizeId);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDto.Quantity;
                existingItem.TotalPriceForOneItemType = existingItem.Product.PriceAfterDiscount * existingItem.Quantity;
                await cartItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                cartItemDto.TotalPriceForOneItemType = cartItemDto.UnitPrice * cartItemDto.Quantity;
                var newCartItem = mapper.Map<CartItem>(cartItemDto);
                newCartItem.CartId = cart.Id;
                await cartItemRepo.AddAsync(newCartItem);
            }

            cartWithItems.TotalBasePrice += itemTotal;
            cartWithItems.TotalAmount = cartWithItems.TotalBasePrice - discountValue;
            cartWithItems.UpdatedAt = DateTime.Now;

            await cartRepo.UpdateAsync(cartWithItems);
            await _unitofwork.SaveChangesAsync();

            var finalItem = await cartItemRepo.GetFirstOrDefaultAsync(
                i => i.CartId == cart.Id &&
                     i.ProductId == cartItemDto.ProductId &&
                     i.ProductSizeId == cartItemDto.ProductSizeId
            );

            return mapper.Map<CartItemDto>(finalItem);
        }



        public async Task<CartItemDto> UpdateCartItem(CartItemDto cartItemDto)
        {
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();
            var cartRepo = _unitofwork.GetBaseRepository<Cart>();

            var userId = _currentUserService.UserId;

            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                ci => ci.Id == cartItemDto.Id,
                include: q => q.Include(ci => ci.ProductSize).ThenInclude(ps => ps.Product)
            );

            if (cartItem == null || cartItem.ProductSize == null || cartItem.ProductSize.Product == null)
                throw new Exception("Cart item or related product not found");

            if (cartItemDto.Quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            if (cartItem.ProductSize.StockQuantity < cartItemDto.Quantity)
                throw new Exception("Insufficient stock");

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.Id == cartItem.CartId,
                include: q => q.Include(c => c.CartItems)
            );

            if (cart == null || cart.UserId != userId)
                throw new Exception("Cart not found for current user");

            // ✅ احسب الفرق في السعر وعدّل العنصر
            var oldTotal = cartItem.TotalPriceForOneItemType;
            cartItem.Quantity = cartItemDto.Quantity;
            cartItem.TotalPriceForOneItemType = cartItem.Product.PriceAfterDiscount * cartItemDto.Quantity;

            await cartItemRepo.UpdateAsync(cartItem);

            // ✅ عدّل الكارت بناءً على الفرق
            cart.TotalBasePrice = cart.TotalBasePrice - oldTotal + cartItem.TotalPriceForOneItemType;

            var discount = await _db.Discounts.FirstOrDefaultAsync();
            var discountValue = discount?.DicountValue ?? 0;

            cart.TotalAmount = cart.TotalBasePrice - (cart.TotalBasePrice * discountValue);
            cart.UpdatedAt = DateTime.Now;

            await cartRepo.UpdateAsync(cart);
            await _unitofwork.SaveChangesAsync();

            return mapper.Map<CartItemDto>(cartItem);
        }

        public async Task<bool> DeleteCartItemFromCart(int cartItemId)
        {
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();
            var cartRepo = _unitofwork.GetBaseRepository<Cart>();

            var userId = _currentUserService.UserId;

            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                ci => ci.Id == cartItemId,
                include: q => q.Include(ci => ci.ProductSize).ThenInclude(ps => ps.Product)
            );

            if (cartItem == null)
                return false;

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.Id == cartItem.CartId,
                include: q => q.Include(c => c.CartItems)
            );

            if (cart == null || cart.UserId != userId)
                return false;

            var itemTotal = cartItem.TotalPriceForOneItemType;

            await cartItemRepo.DeleteAsync(cartItem.Id);

            cart.TotalBasePrice -= itemTotal;

            var discount = await _db.Discounts.FirstOrDefaultAsync();
            var discountValue = discount?.DicountValue ?? 0;

            cart.TotalAmount = cart.TotalBasePrice - (cart.TotalBasePrice * discountValue);
            cart.UpdatedAt = DateTime.Now;

            await cartRepo.UpdateAsync(cart);
            await _unitofwork.SaveChangesAsync();

            return true;
        }

    }


}
