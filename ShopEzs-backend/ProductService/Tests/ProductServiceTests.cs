using Moq;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repositories;
using ProductService.Services;
using Xunit;

namespace ProductService.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepo> _repoMock = new();
    private readonly IProductService _svc;

    public ProductServiceTests() => _svc = new Services.ProductService(_repoMock.Object);

    [Fact]
    public async Task GetAll_ReturnsProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Desc = "Dell Laptop", Price = 50000, Stock = 10 }
        };
        _repoMock.Setup(r => r.GetAll(null, 1, 10)).ReturnsAsync(products);

        var result = await _svc.GetAll(null, 1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsProduct()
    {
        var p = new Product { Id = 1, Name = "Laptop", Desc = "Dell Laptop", Price = 50000, Stock = 10 };
        _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(p);

        var result = await _svc.GetById(1);

        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task GetById_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetById(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _svc.GetById(99));
    }

    [Fact]
    public async Task Add_ValidDto_ReturnsProduct()
    {
        var dto = new ProductDto { Name = "Mobile", Desc = "Samsung Mobile", Price = 20000, Stock = 15 };
        _repoMock.Setup(r => r.Add(It.IsAny<Product>())).ReturnsAsync(2);

        var result = await _svc.Add(dto);

        Assert.Equal(2, result.Id);
        Assert.Equal("Mobile", result.Name);
    }

    [Fact]
    public async Task Delete_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetById(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _svc.Delete(99));
    }

    [Fact]
    public async Task GetAll_WithSearch_ReturnsFilteredProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Desc = "Dell Laptop", Price = 50000, Stock = 10 }
        };
        _repoMock.Setup(r => r.GetAll("Laptop", 1, 10)).ReturnsAsync(products);

        var result = await _svc.GetAll("Laptop", 1, 10);

        Assert.Single(result);
        Assert.Equal("Laptop", result.First().Name);
    }
}
