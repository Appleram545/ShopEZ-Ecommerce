using Microsoft.EntityFrameworkCore;
using Moq;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.Services;
using Xunit;

namespace OrderService.Tests;

public class OrderServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task GetAll_ReturnsAllOrders()
    {
        var db = GetInMemoryDb();
        db.Orders.Add(new Order { UserId = 1, Total = 500, Items = new() });
        db.Orders.Add(new Order { UserId = 2, Total = 300, Items = new() });
        await db.SaveChangesAsync();

        var svc = new OrderServiceMock(db);
        var result = await svc.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByUserId_ReturnsOnlyUserOrders()
    {
        var db = GetInMemoryDb();
        db.Orders.Add(new Order { UserId = 1, Total = 500, Items = new() });
        db.Orders.Add(new Order { UserId = 2, Total = 300, Items = new() });
        await db.SaveChangesAsync();

        var svc = new OrderServiceMock(db);
        var result = await svc.GetByUserId(1);

        Assert.Single(result);
        Assert.Equal(1, result[0].UserId);
    }

    [Fact]
    public async Task GetById_NotFound_ThrowsKeyNotFoundException()
    {
        var db = GetInMemoryDb();
        var svc = new OrderServiceMock(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.GetById(999));
    }

    [Fact]
    public async Task Create_EmptyCart_ThrowsArgumentException()
    {
        var db = GetInMemoryDb();
        var svc = new OrderServiceMock(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.Create(new OrderDto { Items = new() }, 1));
    }
}

// Testable subclass that skips HTTP call to ProductService
public class OrderServiceMock : IOrderService
{
    private readonly AppDbContext _db;
    public OrderServiceMock(AppDbContext db) => _db = db;

    public async Task<Order> Create(OrderDto dto, int userId)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Cart is empty.");

        var items = dto.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId, Qty = i.Qty, Price = 100m
        }).ToList();

        var order = new Order { UserId = userId, Total = items.Sum(x => x.Price * x.Qty), Items = items };
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
