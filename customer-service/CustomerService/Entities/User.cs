namespace CustomerService.Entities;

public class User
{
    public int Id {get;set;}
    public string FullName {get;set;} = string.Empty; //boş başlasın ama null olmasın
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}