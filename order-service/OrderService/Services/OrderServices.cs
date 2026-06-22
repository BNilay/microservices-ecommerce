using OrderService.Entities;
using OrderService.DTOs;
using OrderService.Data;
using Microsoft.EntityFrameworkCore;
using OrderService.Clients;

namespace OrderService.Services;

public class OrderServices
{
    private readonly AppDbContext _context;
    private readonly CustomerHttpClient _customerHttpClient;
    private readonly ProductHttpClient _productHttpClient;
   public OrderServices(
        AppDbContext context,
        CustomerHttpClient customerHttpClient,
        ProductHttpClient productHttpClient)
    {
        _context = context;
        _customerHttpClient = customerHttpClient;
        _productHttpClient = productHttpClient;
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

    public async Task<OrderDto?> CreateOrder(CreateOrderDto createOrderDto)
{
    var customerExists = await _customerHttpClient.CustomerExists(createOrderDto.CustomerId);

    if (!customerExists)
    {
        return null;
    }

    if (createOrderDto.Items == null || !createOrderDto.Items.Any())
    {
        return null;
    }

    var orderItems = new List<OrderItem>();

    foreach (var item in createOrderDto.Items)
    {
        if (item.Quantity <= 0)
        {
            return null;
        }

        var product = await _productHttpClient.GetProductById(item.ProductId);

        if (product == null)
        {
            return null;
        }

        if (product.StockQuantity < item.Quantity)
        {
            return null;
        }

        var orderItem = new OrderItem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = product.Price
        };

        orderItems.Add(orderItem);
    }

    var order = new Order
    {
        CustomerId = createOrderDto.CustomerId,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        Items = orderItems,
        TotalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice)
    };

    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    foreach (var item in order.Items)
    {
        await _productHttpClient.DecreaseStock(item.ProductId, item.Quantity);
    }

    return new OrderDto
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => new OrderItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };

  
}
    public async Task<bool> DeleteOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<OrderDto?> UpdateOrderStatus(int id, OrderStatus newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return null;
        }

        if (order.Status == OrderStatus.Cancelled && newStatus == OrderStatus.Cancelled)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        if (newStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
        {
            foreach (var item in order.Items)
            {
                var stockUpdated = await _productHttpClient.IncreaseStock(item.ProductId, item.Quantity);

                if (!stockUpdated)
                {
                    return null;
                }
            }
        }

        order.Status = newStatus;

        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        // Pending = 0
        //Confirmed = 1
        //Shipped = 2
        //Delivered = 3
        //Cancelled = 4
    }
    public async Task<List<OrderDto>> GetOrdersByCustomerId(int customerId)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
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
}