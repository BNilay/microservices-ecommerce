using OrderService.Entities;

namespace OrderService.DTOs;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}