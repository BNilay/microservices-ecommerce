using System.Net.Http.Json;
using OrderService.DTOs;

namespace OrderService.Clients;

public class PaymentHttpClient
{
    private readonly HttpClient _httpClient;

    public PaymentHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentResultDto?> CreatePayment(CreatePaymentDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Payments", dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payment = await response.Content.ReadFromJsonAsync<PaymentResultDto>();

        return payment;
    }
}