using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Entities;
using PaymentService.Dtos;
using PaymentService.Entities;

namespace PaymentService.Services;

public class PaymentServices
{
    private readonly AppDbContext _context;

    public PaymentServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDto?> CreatePayment(CreatePaymentDto dto)
    {
        if (dto.Amount <= 0)
        {
            return null;
        }

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            Amount = dto.Amount,
            Method = dto.Method,
            Status = PaymentStatus.Complete,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status,
            Method = payment.Method,
            CreatedAt = payment.CreatedAt
        };
    }
}