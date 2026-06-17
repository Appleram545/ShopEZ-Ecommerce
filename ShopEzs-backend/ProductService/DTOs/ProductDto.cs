using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public class ProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Desc{ get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
}