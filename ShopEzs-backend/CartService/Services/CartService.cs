using CartService.DTOs;
using CartService.Models;
using System.Net.Http.Json;

namespace CartService.Services;

// Cart stored in-memory 
public class CartService : ICartService
{
    private static readonly Dictionary<int, Cart> _carts = new();
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private readonly HttpClient _httpClient;
    private readonly string _productServiceUrl;

    public CartService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _productServiceUrl = config["Services:ProductService"]!;
    }

    public Cart GetCart(int userId)
    {
        _carts.TryGetValue(userId, out var cart);
        return cart ?? new Cart { UserId = userId };
    }

    public async Task<Cart> AddItem(int userId, CartItemDto dto)
    {
        var product = await _httpClient.GetFromJsonAsync<ProductDto>($"{_productServiceUrl}/api/product/{dto.ProductId}")
            ?? throw new KeyNotFoundException($"Product {dto.ProductId} not found.");

        if (product.Stock < dto.Qty)
            throw new InvalidOperationException("Insufficient stock.");

        await _lock.WaitAsync();
        try
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
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Qty = dto.Qty
                });

            return cart;
        }
        finally { _lock.Release(); }
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
