﻿using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<CartItemDto> AddCartItemToCurrentCart(CartItemDto cartItemDto);
        Task<CartItemDto> UpdateCartItem(CartItemDto cartItemDto);
        Task<bool> DeleteCartItemFromCart(int Id);
        Task<List<CartItemDto>> AddCartItemsToCurrentCart(List<CartItemDto> cartItemDtos);

    }
}