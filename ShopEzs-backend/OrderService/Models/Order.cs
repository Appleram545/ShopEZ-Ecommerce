namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public decimal Total { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}