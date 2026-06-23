using PaymentService.Entities;

namespace PaymentService.Entities;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public PaymentMethod Method { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}