using AgroRegulate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroRegulate.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WarehouseController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public WarehouseController(IApplicationDbContext context)
    {
        _context = context;
    }

    // Depo Stokları: Anlık mevcut stoklar (hem admin hem şirket kullanıcıları görür)
    [HttpGet("stocks")]
    public async Task<IActionResult> GetStocks()
    {
        var stocks = await _context.WarehouseStocks
            .OrderBy(s => s.ProductType)
            .Select(s => new
            {
                id = s.Id,
                productType = (int)s.ProductType,
                productName = s.ProductType.ToString(),
                availableTons = s.AvailableTons,
                updatedAt = s.UpdatedAt
            })
            .ToListAsync();

        return Ok(stocks);
    }
}
