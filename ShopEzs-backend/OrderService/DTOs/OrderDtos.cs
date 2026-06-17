using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs;

public class OrderDto
{
    [Required, MinLength(1)]
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    [Required, Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required, Range(1, 100)]
    public int Qty { get; set; }
}


public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Desc { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
}
