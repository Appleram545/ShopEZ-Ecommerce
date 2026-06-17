using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
using System.Net.Http.Json;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _httpClient;
    private readonly string _productServiceUrl;

    public OrderService(AppDbContext db, HttpClient httpClient, IConfiguration config)
    {
        _db = db;
        _httpClient = httpClient;
        _productServiceUrl = config["Services:ProductService"]!;
    }

    public async Task<Order> Create(OrderDto dto, int userId)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Cart is empty.");

        var items = new List<OrderItem>();

        foreach (var i in dto.Items)
        {
            // REST call to ProductService to validate & fetch price
            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"{_productServiceUrl}/api/product/{i.ProductId}")
                ?? throw new KeyNotFoundException($"Product {i.ProductId} not found.");

            if (product.Stock < i.Qty)
                throw new InvalidOperationException($"Insufficient stock for product {i.ProductId}.");

            items.Add(new OrderItem { ProductId = i.ProductId, Qty = i.Qty, Price = product.Price });
        }

        var order = new Order
        {
            UserId = userId,
            Date = DateTime.UtcNow,
            Total = items.Sum(x => x.Price * x.Qty),
            Items = items
        };
        foreach (var i in dto.Items)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductDto>(
                $"{_productServiceUrl}/api/product/{i.ProductId}"
            );

            if (product != null)
            {
                product.Stock -= i.Qty;

                var updateDto = new
                {
                    name = product.Name,
                    desc = product.Desc,
                    price = product.Price,
                    stock = product.Stock,
                    imageUrl = product.ImageUrl,
                    category = product.Category
                };

                await _httpClient.PutAsJsonAsync(
                    $"{_productServiceUrl}/api/product/{i.ProductId}",
                    updateDto
                );
            }
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> GetAll()
        => await _db.Orders.Include(o => o.Items).ToListAsync();

    public async Task<List<Order>> GetByUserId(int userId)
        => await _db.Orders.Where(o => o.UserId == userId).Include(o => o.Items).ToListAsync();

    public async Task<Order> GetById(int id)
        => await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id)
           ?? throw new KeyNotFoundException("Order not found.");
}
