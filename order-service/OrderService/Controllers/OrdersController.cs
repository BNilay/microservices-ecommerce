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
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetOrdersByCustomerId(int customerId)
    {
        var orders = await _orderservices.GetOrdersByCustomerId(customerId);

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

    /*[HttpGet("test-customer/{customerId}")]
    public async Task<IActionResult> TestCustomer(int customerId)
    {
    
        var connected = await _customerHttpClient.TestConnection();

        if (!connected)
        {
            return NotFound("CustomerService'e ulaşılamadı.");
        }

        return Ok("CustomerService bağlantısı başarılı.");
    }
    */
    [HttpGet("test-customer/{customerId}")]
    public async Task<IActionResult> TestCustomer(int customerId)
    {
        var exists = await _customerHttpClient.CustomerExists(customerId);

        if (!exists)
        {
            return NotFound("Customer bulunamadı veya CustomerService'e ulaşılamadı.");
        }

        return Ok("CustomerService bağlantısı başarılı. Customer bulundu.");
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
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var order = await _orderservices.CreateOrder(createOrderDto);

        if (order == null)
        {
            return BadRequest("Sipariş oluşturulamadı. Customer, ürün veya stok bilgilerini kontrol et.");
        }

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var result = await _orderservices.DeleteOrder(id);

        if (!result)
        {
            return NotFound("Sipariş bulunamadı."); 
        }

        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var order = await _orderservices.UpdateOrderStatus(id, dto.Status);

        if (order == null)
        {
            return BadRequest("Sipariş durumu güncellenemedi.");
        }

        return Ok(order);
    }

}