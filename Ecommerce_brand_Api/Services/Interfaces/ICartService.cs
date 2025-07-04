﻿using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface ICartService :IBaseService<Cart>
    {

        Task<IEnumerable<CartDto>> GetAllCartsAsync();

        Task<CartDto?> GetCurrentUserCartAsync();

        Task<CartDto?> GetUserCartAsyncById(string userId);

        Task<bool> ClearCurrentUserCart();

        Task<CartDto> AddNewCartAsync(CartDto cart);

        Task<bool> UpdateCartAsync(int Id, CartDto cart);

        Task<bool> DeleteCartAsync(int Id);
        Task<ServiceResult> RemovePurchasedProductsFromCartAsync(string userId, List<int> purchasedProductIds);
    }
}
