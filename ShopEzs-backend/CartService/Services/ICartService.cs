using CartService.DTOs;
using CartService.Models;

namespace CartService.Services;

public interface ICartService
{
    Cart GetCart(int userId);
    Task<Cart> AddItem(int userId, CartItemDto dto);
    Cart RemoveItem(int userId, int productId);
    Cart Clear(int userId);
}
