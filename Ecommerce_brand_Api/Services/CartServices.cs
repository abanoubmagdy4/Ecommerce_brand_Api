﻿
using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services
{
    public class CartServices : BaseService<Cart>, ICartService
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper mapper;
        private readonly ICurrentUserService _currentUserService;


        public CartServices(IUnitofwork unitofwork, IMapper mapper, ICurrentUserService currentUserService) : base(unitofwork.GetBaseRepository<Cart>())


        {
            this._unitofwork = unitofwork;
            this.mapper = mapper;
            this._currentUserService = currentUserService;
        }

        public async Task<IEnumerable<CartDto>> GetAllCartsAsync()
        {
            try
            {
                var cartRepo = _unitofwork.GetBaseRepository<Cart>();
                var carts = await cartRepo.GetAllAsync();
                return mapper.Map<IEnumerable<CartDto>>(carts);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all orders.", ex);
            }
        }

        public async Task<CartDto?> GetCurrentUserCartAsync()
        {
            try
            {
                var cartRepo = _unitofwork.GetBaseRepository<Cart>();

                var cart = await cartRepo.GetFirstOrDefaultAsync(
                     c => c.UserId == _currentUserService.UserId,
            include: q => q
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImagesPaths)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSize)
        );

                if (cart == null)
                {
                    return null;
                }

                var cartDto = mapper.Map<CartDto>(cart);

                var cartItemDtosById = cartDto.CartItems.ToDictionary(ci => ci.Id);

                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItemDtosById.TryGetValue(cartItem.Id, out var cartItemDto))
                    {
                        cartItemDto.ProductImageUrl = cartItem.Product.ProductImagesPaths.FirstOrDefault()?.ImagePath ?? string.Empty;
                        cartItemDto.ProductSizeName = cartItem.ProductSize?.Size ?? "غير محدد";
                    }
                }


                var discount = await _unitofwork.GetBaseRepository<Discount>().GetFirstOrDefaultAsync();

                if (discount == null)
                {
                    cartDto.Threshold = 0;
                    cartDto.TotalDiscount = 0;
                }
                else
                {
                    cartDto.Threshold = discount.Threshold;
                    cartDto.TotalDiscount = cartDto.TotalBasePrice * discount.DicountValue / 100;
                }

                return cartDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving the cart.", ex);
            }
        }

        public async Task<CartDto> AddNewCartAsync(CartDto cartDto)
        {
            if (cartDto == null)
                throw new ArgumentNullException(nameof(cartDto), "Cart data cannot be null.");

            try
            {
                var cart = new Cart
                {
                    UserId = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    TotalBasePrice = cartDto.TotalBasePrice,
                    TotalAmount = cartDto.TotalAmount,
                    CartItems = cartDto.CartItems?.Select(ci => new CartItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        TotalPriceForOneItemType = ci.TotalPriceForOneItemType
                    }).ToList() ?? new List<CartItem>()
                };

                var cartRepo = _unitofwork.GetBaseRepository<Cart>();
                await cartRepo.AddAsync(cart);
                await _unitofwork.SaveChangesAsync();

                var result = new CartDto
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt,
                    TotalBasePrice = cart.TotalBasePrice,
                    TotalAmount = cart.TotalAmount,
                    CartItems = cart.CartItems?.Select(ci => new CartItemDto
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price,
                        TotalPriceForOneItemType = ci.TotalPriceForOneItemType
                    }).ToList()
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new Cart.", ex);
            }
        }

        public async Task<bool> DeleteCartAsync(int Id)
        {
            if (Id <= 0)
                throw new ArgumentException("Invalid Order ID", nameof(Id));

            try
            {
                var repo = _unitofwork.GetBaseRepository<Cart>();

                var deleteCart = await repo.GetByIdAsync(Id);

                if (deleteCart == null)
                {
                    return false;
                }

                await repo.DeleteAsync(Id);
                await _unitofwork.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the Cart with ID {Id}.", ex);
            }
        }

        /// <summary>
        /// Updates an existing cart with the specified ID using the provided updated cart data.
        /// </summary>
        /// <remarks>This method retrieves the cart by its ID, updates its data using the provided
        /// <paramref name="updatedcart"/>,  and saves the changes to the database. If the cart with the specified ID
        /// does not exist, the method returns <see langword="false"/>.</remarks>
        /// <param name="Id">The unique identifier of the cart to update. Must be greater than 0.</param>
        /// <param name="updatedcart">An object containing the updated cart data. Cannot be null.</param>
        /// <returns><see langword="true"/> if the cart was successfully updated; otherwise, <see langword="false"/> if the cart
        /// with the specified ID does not exist.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="Id"/> is less than or equal to 0.</exception>
        /// <exception cref="ApplicationException">Thrown if an error occurs while updating the cart, with additional details about the failure.</exception> 
        public async Task<bool> UpdateCartAsync(int Id, CartDto updatedcart)
        {

            if (Id <= 0)
                throw new ArgumentException("Invalid Order ID", nameof(Id));

            try
            {
                var repo = _unitofwork.GetBaseRepository<Cart>();
                var cart = await repo.GetByIdAsync(Id);

                if (cart == null)
                    return false;

                var updatedEntity = mapper.Map<Cart>(updatedcart);
                updatedEntity.Id = Id;

                await repo.UpdateAsync(updatedEntity);
                await _unitofwork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the order with ID {Id}.", ex);
            }
        }


        public async Task<bool> ClearCurrentUserCart()
        {
            var cartRepo = _unitofwork.GetBaseRepository<Cart>();
            var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();

            var userId = _currentUserService.UserId;

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.UserId == userId,
                include: q => q.Include(c => c.CartItems)
            );

            if (cart == null)
                return false;

            // احذف كل العناصر من الكارت
            await cartItemRepo.DeleteRangeAsync(cart.CartItems);

            // صفّر التوتال
            cart.TotalBasePrice = 0;
            cart.TotalAmount = 0;
            cart.UpdatedAt = DateTime.UtcNow;

            await cartRepo.UpdateAsync(cart);
            await _unitofwork.SaveChangesAsync();

            return true;
        }

        public async Task<CartDto?> GetUserCartAsyncById(string userId)
        {
            try
            {
                var cartRepo = _unitofwork.GetBaseRepository<Cart>();

                var cart = await cartRepo.GetFirstOrDefaultAsync(
                    c => c.UserId == userId,
                    include: q => q.Include(c => c.CartItems)
                );

                return cart == null ? null : mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving the cart.", ex);
            }
        }


        public async Task<ServiceResult> RemovePurchasedProductsFromCartAsync(string userId, List<int> purchasedProductIds)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("Invalid User ID");

            try
            {
                var cartRepo = _unitofwork.Carts;
                var userCart = await cartRepo.GetCartByUserIdAsync(userId);

                if (userCart == null || userCart.CartItems == null || !userCart.CartItems.Any())
                    return ServiceResult.OkWithData(new List<int>()); // مفيش كارت أو مفيش عناصر

                var cartItemRepo = _unitofwork.GetBaseRepository<CartItem>();

                // فلتر المنتجات اللي فعلاً موجودة في الكارت
                var itemsToRemove = userCart.CartItems
                    .Where(ci => purchasedProductIds.Contains(ci.ProductId))
                    .ToList();

                if (!itemsToRemove.Any())
                    return ServiceResult.Ok("No matching products found in cart.");

                await cartItemRepo.AddRangeAsync(itemsToRemove);
                await _unitofwork.SaveChangesAsync();

                var deletedProductIds = itemsToRemove.Select(i => i.ProductId).ToList();

                return ServiceResult.OkWithData(deletedProductIds);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"An error occurred while clearing purchased items from cart for user {userId}. Error: {ex.Message}");
            }
        }


    }
}
