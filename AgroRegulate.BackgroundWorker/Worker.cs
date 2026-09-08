using System.Text.Json;
using AgroRegulate.Application.Features.Allocations.Commands;
using AgroRegulate.Application.Interfaces;
using AgroRegulate.Domain.Entities;
using AgroRegulate.Domain.Enums;

namespace AgroRegulate.BackgroundWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[KAFKA CONSUMER WORKER] Arka plan servisi baþlatýldý. Dinleniyor...");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Kafka Kuyruðundan mesaj gelmiþ gibi simülasyon/dinleme döngüsü
            // Gerçek ortamda burada Consumer.Consume() metodu çalýþýr.

            _logger.LogInformation("[KAFKA CONSUMER] Kuyruk dinleniyor... (Topic: allocation-requests-topic)");

            // 10 saniyede bir kuyruktan veri iþleme simülasyonu
            await Task.Delay(10000, stoppingToken);
        }
    }

    public async Task ProcessAllocationRequestAsync(CreateAllocationRequestCommand command)
    {
        // BackgroundWorker'da Scoped DbContext eriþimi saðlamak için Scope açýyoruz
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var allocationEntity = new AllocationRequest
            {
                AnnouncementId = command.AnnouncementId,
                FactoryId = command.FactoryId,
                RequestedTons = command.RequestedTons,
                Status = RequestStatus.Onaylandi,
                RequestDate = DateTime.UtcNow
            };

            dbContext.AllocationRequests.Add(allocationEntity);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"[DATABASE SUCCESS] Tahsis talebi veritabanýna baþarýyla kaydedildi! ID: {allocationEntity.Id}");
        }
    }
}