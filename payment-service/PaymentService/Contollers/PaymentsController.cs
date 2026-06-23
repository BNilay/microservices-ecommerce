using Microsoft.AspNetCore.Mvc;
using PaymentService.Dtos;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentServices _paymentServices;

    public PaymentsController(PaymentServices paymentServices)
    {
        _paymentServices = paymentServices;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        var payment = await _paymentServices.CreatePayment(dto);

        if (payment == null)
        {
            return BadRequest("Ödeme oluşturulamadı. Tutar 0'dan büyük olmalıdır.");
        }

        return Ok(payment);
    }
}