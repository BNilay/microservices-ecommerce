using System.Net.Http.Json;
using OrderService.DTOs;
namespace OrderService.Clients;

public class ProductHttpClient
{
    private readonly HttpClient _httpClient;
    
    public ProductHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //ProductExists(int productId)

    public async Task<bool> ProductExists(int productId)
    {

        var response = await _httpClient.GetAsync($"/api/Product/{productId}");

        return response.IsSuccessStatusCode;
    }

    public async Task<ProductInfoDto?> GetProductById(int productId)
    {
        var product = await _httpClient.GetFromJsonAsync<ProductInfoDto>($"/api/Product/{productId}");

        return product;
    }
    public async Task<bool> DecreaseStock(int productId, int quantity)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/Product/{productId}/stock", new
        {
            Quantity = -quantity
        });

        return response.IsSuccessStatusCode;
    }
    public async Task<bool> IncreaseStock(int productId, int quantity)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/Product/{productId}/stock", new
        {
            Quantity = quantity
        });

        return response.IsSuccessStatusCode;
    }
    
}