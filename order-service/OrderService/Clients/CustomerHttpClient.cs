namespace OrderService.Clients;

public class CustomerHttpClient
{
    private readonly HttpClient _httpClient;

    public CustomerHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CustomerExists(int customerId)
    {
        var response = await _httpClient.GetAsync($"/api/Customers/{customerId}");
        //c->C

        return response.IsSuccessStatusCode; //HTTP status code 200-299 arasındaysa true dön.

    }
    public async Task<bool> TestConnection()
    {
        var response = await _httpClient.GetAsync("/api/Test");

        return response.IsSuccessStatusCode;
    }

}