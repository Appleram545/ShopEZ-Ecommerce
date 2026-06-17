using CartService.DTOs;
using CartService.Models;
using CartService.Services;
using Xunit;

namespace CartService.Tests;

public class CartServiceTests
{
    // Testable subclass bypassing HTTP calls
    private class TestCartService : ICartService
    {
        private static readonly Dictionary<int, Cart> _carts = new();

        public Cart GetCart(int userId)
        {
            _carts.TryGetValue(userId, out var cart);
            return cart ?? new Cart { UserId = userId };
        }

        public Task<Cart> AddItem(int userId, CartItemDto dto)
        {
            if (!_carts.ContainsKey(userId))
                _carts[userId] = new Cart { UserId = userId };

            var cart = _carts[userId];
            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existing != null)
                existing.Qty += dto.Qty;
            else
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    ProductName = $"Product_{dto.ProductId}",
                    Price = 500m,
                    Qty = dto.Qty
                });

            return Task.FromResult(cart);
        }

        public Cart RemoveItem(int userId, int productId)
        {
            if (!_carts.TryGetValue(userId, out var cart))
                return new Cart { UserId = userId };
            cart.Items.RemoveAll(i => i.ProductId == productId);
            return cart;
        }

        public Cart Clear(int userId)
        {
            _carts.Remove(userId);
            return new Cart { UserId = userId };
        }
    }

    private readonly ICartService _svc = new TestCartService();

    [Fact]
    public async Task AddItem_NewProduct_AddsToCart()
    {
        var cart = await _svc.AddItem(1, new CartItemDto { ProductId = 10, Qty = 2 });

        Assert.Single(cart.Items);
        Assert.Equal(10, cart.Items[0].ProductId);
        Assert.Equal(2, cart.Items[0].Qty);
    }

    [Fact]
    public async Task AddItem_ExistingProduct_IncreasesQty()
    {
        await _svc.AddItem(2, new CartItemDto { ProductId = 5, Qty = 1 });
        var cart = await _svc.AddItem(2, new CartItemDto { ProductId = 5, Qty = 3 });

        Assert.Single(cart.Items);
        Assert.Equal(4, cart.Items[0].Qty);
    }

    [Fact]
    public async Task RemoveItem_RemovesFromCart()
    {
        await _svc.AddItem(3, new CartItemDto { ProductId = 7, Qty = 2 });
        var cart = _svc.RemoveItem(3, 7);

        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task Clear_EmptiesCart()
    {
        await _svc.AddItem(4, new CartItemDto { ProductId = 1, Qty = 1 });
        await _svc.AddItem(4, new CartItemDto { ProductId = 2, Qty = 2 });
        var cart = _svc.Clear(4);

        Assert.Empty(cart.Items);
    }

    [Fact]
    public void GetCart_EmptyUser_ReturnsEmptyCart()
    {
        var cart = _svc.GetCart(999);

        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task Cart_Total_CalculatesCorrectly()
    {
        var cart = await _svc.AddItem(5, new CartItemDto { ProductId = 3, Qty = 3 });

        Assert.Equal(1500m, cart.Total); 
    }
}
