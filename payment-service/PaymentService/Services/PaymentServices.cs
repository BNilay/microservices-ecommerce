using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
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

        var isFailed = Random.Shared.Next(1, 101) <= 20;

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            Amount = dto.Amount,
            Method = dto.Method,
            Status = isFailed ? PaymentStatus.Failed : PaymentStatus.Complete,
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

    public async Task<PaymentDto?> GetPaymentById(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
        {
            return null;
        }

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

    public async Task<PaymentDto?> RefundPayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
        {
            return null;
        }

        if (payment.Status != PaymentStatus.Complete)
        {
            return null;
        }

        payment.Status = PaymentStatus.Refunded;

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