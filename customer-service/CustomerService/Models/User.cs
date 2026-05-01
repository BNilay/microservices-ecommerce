namespace CustomerService.Models;

public class User
{
    public int Id {get;set;}
    public string FullName {get;set;} = string.Empty; //boş başlasın ama null olmasın
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}