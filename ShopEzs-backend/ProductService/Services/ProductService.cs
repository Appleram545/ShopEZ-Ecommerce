using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _repo;

    public ProductService(IProductRepo repo) => _repo = repo;

    public Task<IEnumerable<Product>> GetAll(string? search, int page, int pageSize)
        => _repo.GetAll(search, page, pageSize);

    public async Task<Product> GetById(int id)
    {
        var p = await _repo.GetById(id);
        return p ?? throw new KeyNotFoundException($"Product {id} not found.");
    }

    public async Task<Product> Add(ProductDto dto)
    {
        var p = new Product
        {
            Name = dto.Name,
            Desc = dto.Desc,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            Category = dto.Category
        };

        p.Id = await _repo.Add(p);

        return p;
    }

    public async Task<Product> Update(int id, ProductDto dto)
    {
        var p = await GetById(id);

        p.Name = dto.Name;
        p.Desc = dto.Desc;
        p.Price = dto.Price;
        p.Stock = dto.Stock;
        p.ImageUrl = dto.ImageUrl;
        p.Category = dto.Category;

        await _repo.Update(p);

        return p;
    }

    public async Task<bool> Delete(int id)
    {
        await GetById(id);

        await _repo.Delete(id);

        return true;
    }
}