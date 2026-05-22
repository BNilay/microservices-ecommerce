using Microsoft.EntityFrameworkCore;
using OrderService.DTOs;
using OrderService.Data;
using OrderService.Entities;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using OrderService.Clients;


namespace OrderService.Controller;

[ApiController]
[Route("api/[controller]")]

public class OrdersController : ControllerBase
{
    private readonly OrderServices _orderservices;
    private readonly CustomerHttpClient _customerHttpClient;
    private readonly ProductHttpClient _productHttpClient;

    public OrdersController(OrderServices orderservices,CustomerHttpClient customerHttpClient, ProductHttpClient productHttpClient )
    {
        _orderservices = orderservices;
        _customerHttpClient = customerHttpClient;
        _productHttpClient = productHttpClient;
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

    [HttpGet("test-customer/{customerId}")]
    public async Task<IActionResult> TestCustomer(int customerId)
    {
        /*
        var exists = await _customerHttpClient.CustomerExists(customerId);

        if (!exists)
        {
            return NotFound("Customer bulunamadı veya CustomerService'e ulaşılamadı.");
        }

        return Ok("CustomerService bağlantısı başarılı. Customer bulundu.");
        */

        var connected = await _customerHttpClient.TestConnection();

        if (!connected)
        {
            return NotFound("CustomerService'e ulaşılamadı.");
        }

        return Ok("CustomerService bağlantısı başarılı.");
    }

    [HttpGet("test-product/{productId}")]
    public async Task<IActionResult> TestProduct(int productId)
    {
        var exists = await _productHttpClient.ProductExists(productId);

        if (!exists)
        {
            return NotFound("Product bulunamdı veya ProductService'e ulaşılmadı.");
        }

        return Ok("ProductService bağlantısı başarılı. Product bulundu.");
    }
}