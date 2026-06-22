namespace OrderService.DTOs;

public class ProductInfoDto
{
    public int Id{get;set;}
    public string Name{get;set;} = string.Empty;
    public decimal Price{get;set;}
    public int StockQuantity{get;set;}
    public string Category{get;set;}= string.Empty;

}