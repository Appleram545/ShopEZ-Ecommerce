using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs;

public class CartItemDto
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
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
