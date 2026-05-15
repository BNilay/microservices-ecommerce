using Microsoft.EntityFrameworkCore;
using OrderService.DTOs;
using OrderService.Data;
using OrderService.Entities;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Controller;

[ApiController]
[Route("api/[controller]")]

public class OrdersController : ControllerBase
{
    private readonly OrderServices _orderservices;

    public OrdersController(OrderServices orderservices)
    {
        _orderservices = orderservices;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderservices.GetAllOrders();
        
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var orders = await _orderservices.GetOrderById(id);

        if (orders == null)
        {
            return NotFound("Sipariş bulunamadı.");
        }

        return Ok(orders);
        
    }
}