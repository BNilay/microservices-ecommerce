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

  
}