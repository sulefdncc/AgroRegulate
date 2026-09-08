using AgroRegulate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgroRegulate.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class AllocationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AllocationsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("request-allocation")]
    public async Task<IActionResult> RequestAllocation([FromBody] KolayRequestDto request)
    {
        var factoryIdClaim = User.FindFirst("FactoryId")?.Value;
        if (!Guid.TryParse(factoryIdClaim, out var factoryId) || factoryId == Guid.Empty)
        {
            return Unauthorized(new { Message = "Geçerli bir oturum bulunamadı." });
        }

        var factory = await _context.Factories.FindAsync(factoryId);
        if (factory == null) return NotFound(new { Message = "Firma bulunamadı." });

        var newRequest = new AgroRegulate.Domain.Entities.AllocationRequest
        {
            Id = Guid.NewGuid(),
            FactoryId = factoryId,
            ProductType = (AgroRegulate.Domain.Enums.ProductType)request.ProductType,
            RequestedTons = request.RequestedTons,
            RequestDate = DateTime.UtcNow,
            Status = AgroRegulate.Domain.Enums.RequestStatus.Beklemede
        };

        _context.AllocationRequests.Add(newRequest);
        await _context.SaveChangesAsync(default);

        return Ok(new { message = "Talebiniz başarıyla sıraya alındı." });
    }

    [HttpGet("my-requests/{factoryId}")]
    public async Task<IActionResult> GetMyRequests(Guid factoryId)
    {
        var requests = await _context.AllocationRequests
            .Where(x => x.FactoryId == factoryId)
            .OrderByDescending(x => x.RequestDate)
            .Select(x => new {
                id = x.Id,
                productType = (int)x.ProductType,
                productName = x.ProductType.ToString(),
                requestedTons = x.RequestedTons,
                requestDate = x.RequestDate,
                status = (int)x.Status
            })
            .ToListAsync();
        return Ok(requests);
    }

    [HttpGet("pending-requests")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var requests = await (from req in _context.AllocationRequests
                              join fac in _context.Factories on req.FactoryId equals fac.Id
                              where req.Status == AgroRegulate.Domain.Enums.RequestStatus.Beklemede
                              orderby req.RequestDate
                              select new
                              {
                                  id = req.Id,
                                  factoryName = fac.Name,
                                  productType = (int)req.ProductType,
                                  productName = req.ProductType.ToString(),
                                  requestedTons = req.RequestedTons,
                                  requestDate = req.RequestDate
                              }).ToListAsync();
        return Ok(requests);
    }

    [HttpPost("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto request)
    {
        var allocation = await _context.AllocationRequests.FindAsync(id);
        if (allocation == null) return NotFound("Talep bulunamadı.");

        allocation.Status = (AgroRegulate.Domain.Enums.RequestStatus)request.Status;
        await _context.SaveChangesAsync(default);

        return Ok(new { message = "Talep durumu güncellendi." });
    }

    // --- YENİ EKLENEN KOTA HESAPLAMA METODU ---
    [HttpGet("quota")]
    public async Task<IActionResult> GetQuota()
    {
        var factoryIdClaim = User.FindFirst("FactoryId")?.Value;
        if (!Guid.TryParse(factoryIdClaim, out var factoryId) || factoryId == Guid.Empty)
            return Ok(new { total = 0, used = 0, remaining = 0 });

        // Reddedilmeyen (Beklemede ve Onaylandı) tüm talepleri kullanılmış kota sayıyoruz
        var usedQuota = await _context.AllocationRequests
            .Where(x => x.FactoryId == factoryId && (int)x.Status != 3)
            .SumAsync(x => x.RequestedTons);

        var total = 1000m;
        var remaining = total - usedQuota;

        return Ok(new { total = total, used = usedQuota, remaining = remaining });
    }
}

public class KolayRequestDto { public decimal RequestedTons { get; set; } public int ProductType { get; set; } }
public class UpdateStatusDto { public int Status { get; set; } }