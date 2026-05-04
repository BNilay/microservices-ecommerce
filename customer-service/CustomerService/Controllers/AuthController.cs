using CustomerService.DTOs;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.Register(dto);

        if (result == null)
            return BadRequest(new { message = "Bu email zaten kayıtlı." });

        return Ok(result);
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.Login(dto);

        if (token == null)
            return Unauthorized(new { message = "Email veya şifre hatalı." });

        // Token'ı döndür — client bunu saklayıp her istekte gönderecek
        return Ok(new { token });
    }

}