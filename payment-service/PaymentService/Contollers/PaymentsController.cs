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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var payment = await _paymentServices.GetPaymentById(id);

        if (payment == null)
        {
            return NotFound("Ödeme bulunamadı.");
        }

        return Ok(payment);
    }

    [HttpPost("{id}/refund")]
    public async Task<IActionResult> RefundPayment(int id)
    {
        var payment = await _paymentServices.RefundPayment(id);

        if (payment == null)
        {
            return BadRequest("Ödeme iade edilemedi. Ödeme bulunamadı veya Completed durumunda değil.");
        }

        return Ok(payment);
    }
    
}