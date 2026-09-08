using AgroRegulate.Domain.Entities;
using AgroRegulate.Domain.Enums;
using AgroRegulate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroRegulate.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static readonly Guid Factory1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Factory2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Announcement1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public static async Task SeedAsync(AgroDbContext context)
    {
        // InMemory test modu: migration uygulanamaz, şema doğrudan oluşturulur
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        if (!await context.Factories.AnyAsync())
        {
            var factories = new List<Factory>
            {
                new Factory
                {
                    Id = Factory1Id,
                    Name = "Anadolu Un ve Yem Sanayi A.Ş.",
                    Address = "Ankara Organize Sanayi Bölgesi, 1. Cadde No:5, Ankara",
                    TaxNumber = "1234567890",
                    PasswordHash = PasswordHasher.Hash("123456"),
                    SectorType = SectorType.Yem,
                    AccountStatus = AccountStatus.Approved,
                    IsActive = true
                },
                new Factory
                {
                    Id = Factory2Id,
                    Name = "Trakya Nişasta ve Gıda San. Tic.",
                    Address = "Trakya Serbest Bölgesi, 3. Parsel, Tekirdağ",
                    TaxNumber = "9876543210",
                    PasswordHash = PasswordHasher.Hash("123456"),
                    SectorType = SectorType.Gida,
                    AccountStatus = AccountStatus.Approved,
                    IsActive = true
                }
            };

            await context.Factories.AddRangeAsync(factories);
        }

        if (!await context.Quotas.AnyAsync())
        {
            var quotas = new List<Quota>
            {
                new Quota
                {
                    Id = Guid.NewGuid(),
                    FactoryId = Factory1Id,
                    ProductType = ProductType.Misir,
                    TotalAllocatedTons = 1000m,
                    RemainingTons = 1000m,
                    Year = DateTime.UtcNow.Year
                },
                new Quota
                {
                    Id = Guid.NewGuid(),
                    FactoryId = Factory2Id,
                    ProductType = ProductType.Misir,
                    TotalAllocatedTons = 500m,
                    RemainingTons = 500m,
                    Year = DateTime.UtcNow.Year
                }
            };

            await context.Quotas.AddRangeAsync(quotas);
        }

        if (!await context.SalesAnnouncements.AnyAsync())
        {
            var announcement = new SalesAnnouncement
            {
                Id = Announcement1Id,
                Title = "2026 Sezonu Yerli Mısır Tahsis İlanı (TMO)",
                ProductType = ProductType.Misir,
                TotalOfferedTons = 5000m,
                AvailableTons = 5000m,
                UnitPrice = 8500m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            await context.SalesAnnouncements.AddAsync(announcement);
        }

        if (!await context.WarehouseStocks.AnyAsync())
        {
            var stocks = new List<WarehouseStock>
            {
                new WarehouseStock { Id = Guid.NewGuid(), ProductType = ProductType.Misir, AvailableTons = 5000m },
                new WarehouseStock { Id = Guid.NewGuid(), ProductType = ProductType.Bugday, AvailableTons = 3000m },
                new WarehouseStock { Id = Guid.NewGuid(), ProductType = ProductType.SekerPancari, AvailableTons = 2000m },
                new WarehouseStock { Id = Guid.NewGuid(), ProductType = ProductType.Arpa, AvailableTons = 1500m }
            };

            await context.WarehouseStocks.AddRangeAsync(stocks);
        }

        await context.SaveChangesAsync();
    }
}