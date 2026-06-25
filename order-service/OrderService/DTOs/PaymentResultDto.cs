
namespace OrderService.DTOs;

public class PaymentResultDto
{
    public int Id{get;set;}
    public int OrderId{get;set;}
    public decimal Amount{get;set;}
    public int Status{get;set;}
    public int Method{get;set;}
    public DateTime CreatedAt{get;set;}
}