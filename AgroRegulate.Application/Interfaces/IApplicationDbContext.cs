using AgroRegulate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroRegulate.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Factory> Factories { get; }
    DbSet<Quota> Quotas { get; }
    DbSet<SalesAnnouncement> SalesAnnouncements { get; }
    DbSet<AllocationRequest> AllocationRequests { get; }
    DbSet<WarehouseStock> WarehouseStocks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}