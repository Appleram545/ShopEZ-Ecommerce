using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAll(string? search, int page, int pageSize);
    Task<Product> GetById(int id);
    Task<Product> Add(ProductDto dto);
    Task<Product> Update(int id, ProductDto dto);
    Task<bool> Delete(int id);
}
