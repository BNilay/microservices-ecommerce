using OrderService.Entities;
using OrderService.DTOs;
using OrderService.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Services;

public class OrderServices
{
    private readonly AppDbContext _context;

    public OrderServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)  //Siparişleri getirirken, her siparişin Items listesini de getir.
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .ToListAsync();

        return orders;
    }

    public async Task<OrderDto?> GetOrderById(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.Id == id)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return order;
    }
}