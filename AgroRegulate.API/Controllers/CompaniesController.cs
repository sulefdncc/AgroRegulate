using AgroRegulate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroRegulate.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public CompaniesController(IApplicationDbContext context)
    {
        _context = context;
    }

    // Şirket Paneli - Profilim: Giriş yapan firmanın bilgileri
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var factoryIdClaim = User.FindFirst("FactoryId")?.Value;
        if (!Guid.TryParse(factoryIdClaim, out var factoryId) || factoryId == Guid.Empty)
            return Unauthorized(new { Message = "Geçerli bir oturum bulunamadı." });

        var factory = await _context.Factories.FindAsync(factoryId);
        if (factory == null) return NotFound(new { Message = "Firma bulunamadı." });

        return Ok(new
        {
            id = factory.Id,
            name = factory.Name,
            address = factory.Address,
            taxNumber = factory.TaxNumber,
            sectorType = (int)factory.SectorType,
            accountStatus = (int)factory.AccountStatus
        });
    }
}
