using PaymentService.Entities;

namespace PaymentService.Dtos;

public class PaymentDto
{
    public int Id{get;set;}
    public int OrderId{get;set;}
    public decimal Amount{get;set;}
    public PaymentStatus Status{get;set;}
    public PaymentMethod Method{get;set;}
    public DateTime CreatedAt{get;set;}




}