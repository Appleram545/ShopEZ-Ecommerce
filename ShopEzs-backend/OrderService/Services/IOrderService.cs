using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<Order> Create(OrderDto dto, int userId);
    Task<List<Order>> GetAll();
    Task<List<Order>> GetByUserId(int userId);
    Task<Order> GetById(int id);
}
