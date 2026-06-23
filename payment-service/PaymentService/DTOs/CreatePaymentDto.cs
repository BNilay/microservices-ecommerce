using PaymentService.Entities;

namespace PaymentService.Dtos;

public class CreatePaymentDto
{
    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }   
}