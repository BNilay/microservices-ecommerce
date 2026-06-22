using CustomerService.Data;
using CustomerService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize] // Bu controller'daki tüm endpoint'ler token ister
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Street = user.Street,
            City = user.City,
            Country = user.Country,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RegisterDto dto)
    {
        //Bu method hemen sonucu döndürmez 
        // //ileride dönecek bir sonucu temsil eder
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        user.FullName = dto.FullName;
        user.Street = dto.Street;
        user.City = dto.City;
        user.Country = dto.Country;

        await _context.SaveChangesAsync();

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Street = user.Street,
            City = user.City,
            Country = user.Country,
            CreatedAt = user.CreatedAt
        });
    }
}