using System.ComponentModel.DataAnnotations;

namespace CartService.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public decimal Subtotal => Price * Qty;
}

public class Cart
{
    public int UserId { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Subtotal);
}
