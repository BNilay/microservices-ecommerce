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

    [HttpGet]
    public async Task<IActionResult> GetAllProduct()
    {
        var products = await _productServices.GetAllProduct();

        return Ok(products);
    }

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

}