using OrderService.Entities;

namespace OrderService.DTOs;

public class OrderDto
{
    public int Id{get;set;}
    public int CustomerId{get;set;}
    public OrderStatus Status { get; set; } 
    public decimal TotalAmount{get;set;}
    public List<OrderItemDto> Items{get;set;} = new(); //liste boş olarak başlar.
    public DateTime CreatedAt { get; set; }
}