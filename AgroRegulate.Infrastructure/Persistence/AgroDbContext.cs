using AgroRegulate.Application.Interfaces;
using AgroRegulate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroRegulate.Infrastructure.Persistence;

public class AgroDbContext : DbContext, IApplicationDbContext
{
    public AgroDbContext(DbContextOptions<AgroDbContext> options) : base(options)
    {
    }

    public DbSet<Factory> Factories => Set<Factory>();
    public DbSet<Quota> Quotas => Set<Quota>();
    public DbSet<SalesAnnouncement> SalesAnnouncements => Set<SalesAnnouncement>();
    public DbSet<AllocationRequest> AllocationRequests => Set<AllocationRequest>();
    public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}