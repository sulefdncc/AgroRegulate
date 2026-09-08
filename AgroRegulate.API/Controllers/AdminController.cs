using AgroRegulate.Application.Interfaces;
using AgroRegulate.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroRegulate.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminController(IApplicationDbContext context)
    {
        _context = context;
    }

    // 1) Şirket Onayları: Onay bekleyen (Pending) firmaların listesi
    [HttpGet("companies/pending")]
    public async Task<IActionResult> GetPendingCompanies()
    {
        var companies = await _context.Factories
            .Where(f => f.AccountStatus == AccountStatus.Pending)
            .OrderBy(f => f.Id)
            .Select(f => new
            {
                id = f.Id,
                name = f.Name,
                address = f.Address,
                taxNumber = f.TaxNumber,
                sectorType = (int)f.SectorType
            })
            .ToListAsync();

        return Ok(companies);
    }

    // 2) Şirket Onayı: Firma onaylanır, giriş yapabilir hale gelir
    [HttpPost("companies/{id:guid}/approve")]
    public async Task<IActionResult> ApproveCompany(Guid id)
    {
        var factory = await _context.Factories.FindAsync(id);
        if (factory == null) return NotFound(new { Message = "Firma bulunamadı." });

        factory.AccountStatus = AccountStatus.Approved;
        factory.IsActive = true;
        await _context.SaveChangesAsync(default);

        return Ok(new { Message = $"{factory.Name} onaylandı." });
    }

    // 3) Şirket Reddi: Firma reddedilir, giriş yapamaz
    [HttpPost("companies/{id:guid}/reject")]
    public async Task<IActionResult> RejectCompany(Guid id)
    {
        var factory = await _context.Factories.FindAsync(id);
        if (factory == null) return NotFound(new { Message = "Firma bulunamadı." });

        factory.AccountStatus = AccountStatus.Rejected;
        factory.IsActive = false;
        await _context.SaveChangesAsync(default);

        return Ok(new { Message = $"{factory.Name} reddedildi." });
    }
}
