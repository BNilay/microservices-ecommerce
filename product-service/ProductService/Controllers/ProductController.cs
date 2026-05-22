using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Services;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductServices _productServices;

    public ProductController(ProductServices productServices)
    {
        _productServices = productServices;
    }

    /// <summary>
    /// Ürünleri listeler.
    /// </summary>
    /// <remarks>
    /// Tüm ürünleri getirir. İsteğe bağlı olarak category veya inStock query parametreleriyle filtreleme yapılabilir.
    /// 
    /// Örnek kullanımlar:
    /// GET /api/Product
    /// GET /api/Product?category=Electronics
    /// GET /api/Product?inStock=true
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetAllProduct([FromQuery] string? category, [FromQuery] bool? inStock)
    {
        if (inStock == true)
        {
            var inStockProducts = await _productServices.GetProductsInStock();
            return Ok(inStockProducts);
        }

        if (!string.IsNullOrEmpty(category))
        {
            var filteredProducts = await _productServices.GetProductsByCategory(category);
            return Ok(filteredProducts);
        }
        var products = await _productServices.GetAllProduct();
    
        return Ok(products);
    }

    /// <summary>
    /// Yeni ürün ekler.
    /// </summary>
    /// <remarks>
    /// Ürün adı, açıklaması, fiyatı, stok miktarı ve kategorisi ile yeni bir ürün oluşturur.
    /// Fiyat veya stok miktarı negatif olamaz.
    /// </remarks>

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        var product = await _productServices.CreateProduct(dto);

        if (product == null)
        {
            return BadRequest("ürün eklenemedi.");
        }

        return Ok(product);
    }

    /// <summary>
    /// Ürün bilgilerini günceller.
    /// </summary>
    /// <remarks>
    /// Belirtilen id değerine sahip ürünün adını, açıklamasını, fiyatını, stok miktarını ve kategorisini günceller.
    /// Ürün bulunamazsa veya geçersiz veri gönderilirse hata döner.
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id , CreateProductDto dto)
    {
        var product = await _productServices.UpdateProduct(id,dto);

        if(product == null)
        {
            return BadRequest("ürün güncellenemedi.");
        }
        return Ok(product);

    }
    /// <summary>
    /// Ürün siler.
    /// </summary>
    /// <remarks>
    /// Belirtilen id değerine sahip ürünü veritabanından siler.
    /// Ürün bulunamazsa 404 Not Found döner.
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productServices.DeleteProduct(id);
        if(!result)
        {
            return NotFound("ürün bulunamadı");
        }
        return NoContent();
    }
    
    /// <summary>
    /// Ürün stok miktarını artırır veya azaltır.
    /// </summary>
    /// <remarks>
    /// Belirtilen ürünün stok miktarını gönderilen quantity değerine göre günceller.
    /// Quantity pozitif ise stok artar, negatif ise stok azalır.
    /// Stok miktarının sıfırın altına düşmesine izin verilmez.
    /// </remarks>
    [HttpPut("{id}/stock")] //endpoint updateproductla aynı olmasın diye stock ekledik
    public async Task<IActionResult> UpdateStock(int id, UpdateStockDto dto)
    {
        var product = await _productServices.UpdateStock(id,dto);
        if (product== null)
        {
            return BadRequest("Ürün bulunamadı veya stok sıfırın altına düşemez.");
        }
        return Ok(product);
    }

    /// <summary>
    /// CustomerService bağlantısını test eder.
    /// </summary>
    /// <remarks>
    /// ProductService içinden HttpClient kullanarak CustomerService'in /api/Test endpointine istek atar.
    /// Docker Compose ortamında servisler arası network bağlantısını doğrulamak için kullanılır.
    /// </remarks>
    [HttpGet("test-customer/{customerId}")]
    public async Task<IActionResult> TestCustomerService(int customerId)
    {
        var customer = await _productServices.GetCustomerFromCustomerService(customerId);

        if (customer == null)
        {
            return NotFound("Customer bulunamadı veya CustomerService'e ulaşılamadı.");
        }

        return Ok(customer);
    }

    [HttpGet("test-customer-service")]
    public async Task<IActionResult> TestCustomerServiceConection()
    {
        var result = await _productServices.TestCustomerServiceConnection();

        if(result == null)
        {
            return BadRequest("CustomerService'e ulaşılamadu.");
        }
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        //ProductServices.GetProductById(id) çağır

        var result = await _productServices.GetProductById(id);
        
        if(result== null)
        {
            return NotFound("Bu id ile ürün bulunamadı");
        }
        return Ok(result);
        
    }
}