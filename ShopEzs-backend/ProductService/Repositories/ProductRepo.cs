using Dapper;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Repositories;

// Uses Dapper for all DB operations 
public class ProductRepo : IProductRepo
{
    private readonly DbConnectionFactory _factory;

    public ProductRepo(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Product>> GetAll(string? search = null, int page = 1, int pageSize = 10)
    {
        using var conn = _factory.CreateConnection();

        var offset = (page - 1) * pageSize;

        // Dapper: search & pagination query
        var sql = string.IsNullOrWhiteSpace(search)
            ? "SELECT * FROM Products ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
            : "SELECT * FROM Products WHERE Name LIKE @Search OR [Desc] LIKE @Search ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        return await conn.QueryAsync<Product>(sql, new
        {
            Search = $"%{search}%",
            Offset = offset,
            PageSize = pageSize
        });
    }

    public async Task<Product?> GetById(int id)
    {
        using var conn = _factory.CreateConnection();

        return await conn.QueryFirstOrDefaultAsync<Product>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }

    public async Task<int> Add(Product p)
    {
        using var conn = _factory.CreateConnection();

        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO Products 
            (Name, [Desc], Price, Stock, ImageUrl, Category) 
            OUTPUT INSERTED.Id 
            VALUES 
            (@Name, @Desc, @Price, @Stock, @ImageUrl, @Category)",
            new
            {
                p.Name,
                p.Desc,
                p.Price,
                p.Stock,
                p.ImageUrl,
                p.Category
            }
        );
    }

    public async Task Update(Product p)
    {
        using var conn = _factory.CreateConnection();

        await conn.ExecuteAsync(
            @"UPDATE Products 
              SET 
                Name = @Name,
                [Desc] = @Desc,
                Price = @Price,
                Stock = @Stock,
                ImageUrl = @ImageUrl,
                Category = @Category
              WHERE Id = @Id",
            new
            {
                p.Name,
                p.Desc,
                p.Price,
                p.Stock,
                p.ImageUrl,
                p.Category,
                p.Id
            }
        );
    }

    public async Task Delete(int id)
    {
        using var conn = _factory.CreateConnection();

        await conn.ExecuteAsync(
            "DELETE FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }
}