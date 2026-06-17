using ProductService.Models;

namespace ProductService.Repositories;

public interface IProductRepo
{
    Task<IEnumerable<Product>> GetAll(string? search = null, int page = 1, int pageSize = 10);
    Task<Product?> GetById(int id);
    Task<int> Add(Product p);
    Task Update(Product p);
    Task Delete(int id);
}
