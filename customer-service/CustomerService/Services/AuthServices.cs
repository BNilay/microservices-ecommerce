using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerService.Data;
using CustomerService.DTOs;
using CustomerService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CustomerService.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    // IConfiguration ile appsettings.json'daki JWT ayarlarını okuyacağız
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<UserResponseDto?> Register(RegisterDto dto)
    {
        // Aynı email kayıtlı mı?
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existing != null) return null;

        // Şifreyi hashle — düz metin asla kaydedilmez
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Street = dto.Street,
            City = dto.City,
            Country = dto.Country
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<string?> Login(LoginDto dto)
    {
        // Kullanıcıyı bul
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return null;

        // Şifreyi doğrula
        var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValid) return null;

        // Doğrulama başarılı — JWT token üret
        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        // Token içine koyacağımız bilgiler (claim)
        // Bu bilgiler token decode edilince görünür — hassas bilgi koyma
        var claims = new[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("fullName", user.FullName)
        };

        // appsettings.json'dan gizli anahtarı oku
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        // İmzalama algoritması — HMAC SHA256 endüstri standardı
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Token'ı oluştur
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // 24 saat geçerli
            signingCredentials: credentials
        );

        // Token'ı string'e çevir
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // User entity'yi UserResponseDto'ya çeviren yardımcı metod
    // Bunu bir kez yazıp her yerde kullanıyoruz — kod tekrarı önlenir
    private UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Street = user.Street,
            City = user.City,
            Country = user.Country,
            CreatedAt = user.CreatedAt
        };
    }
}