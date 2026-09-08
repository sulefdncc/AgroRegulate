using AgroRegulate.Application.Interfaces;
using AgroRegulate.Domain.Entities;
using AgroRegulate.Domain.Enums;
using AgroRegulate.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgroRegulate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _context;

    public AuthController(IConfiguration configuration, IApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    // 1) Yeni Kimlik: Şirket Kaydı (Admin onayı bekler -> Pending)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName) ||
            string.IsNullOrWhiteSpace(request.Address) ||
            string.IsNullOrWhiteSpace(request.TaxNumber) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { Message = "Tüm alanlar zorunludur!" });
        }

        var exists = await _context.Factories.AnyAsync(f => f.TaxNumber == request.TaxNumber.Trim());
        if (exists)
        {
            return BadRequest(new { Message = "Bu VKN ile zaten bir kayıt bulunuyor!" });
        }

        var factory = new Factory
        {
            Name = request.CompanyName.Trim(),
            Address = request.Address.Trim(),
            TaxNumber = request.TaxNumber.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            AccountStatus = AccountStatus.Pending,
            IsActive = false
        };

        _context.Factories.Add(factory);
        await _context.SaveChangesAsync(default);

        return Ok(new { Message = "Kaydınız alındı. Yönetici onayının ardından giriş yapabilirsiniz." });
    }

    // 2) Giriş: Admin yapılandırmadan; Fabrika -> VKN + Şifre
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var adminUsername = _configuration["Admin:Username"];
        var adminPassword = _configuration["Admin:Password"];

        if (!string.IsNullOrWhiteSpace(adminUsername) &&
            request.Username == adminUsername &&
            request.Password == adminPassword)
        {
            var token = GenerateJwtToken(adminUsername, "Admin", Guid.Empty);
            return Ok(new { Token = token, Role = "Admin" });
        }

        // Fabrika girişi: VKN (vergi kimlik no) + şifre
        var factory = await _context.Factories
            .FirstOrDefaultAsync(f => f.TaxNumber == request.Username.Trim());

        if (factory == null || !PasswordHasher.Verify(request.Password, factory.PasswordHash))
        {
            return Unauthorized(new { Message = "VKN veya şifre hatalı!" });
        }

        if (factory.AccountStatus != AccountStatus.Approved)
        {
            return StatusCode(403, new { Message = "Hesabınız henüz onaylanmadı. Yönetici onayı bekleniyor." });
        }

        var factoryToken = GenerateJwtToken(factory.Name, "Factory", factory.Id);
        return Ok(new { Token = factoryToken, Role = "Factory", FactoryId = factory.Id });
    }

    private string GenerateJwtToken(string username, string role, Guid factoryId)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("FactoryId", factoryId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string CompanyName, string Address, string TaxNumber, string Password);