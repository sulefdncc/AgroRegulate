using AgroRegulate.Application.Interfaces;
using MediatR;

namespace AgroRegulate.Application.Features.Allocations.Commands;

// 1. İstek Modeli (Command)
public class CreateAllocationRequestCommand : IRequest<bool>
{
    public Guid AnnouncementId { get; set; }
    public Guid FactoryId { get; set; }
    public decimal RequestedTons { get; set; }
}

// 2. İşleyici (Handler)
public class CreateAllocationRequestHandler : IRequestHandler<CreateAllocationRequestCommand, bool>
{
    private readonly IRedisCacheService _cacheService;
    private readonly IKafkaProducerService _kafkaService;

    public CreateAllocationRequestHandler(
        IRedisCacheService cacheService,
        IKafkaProducerService kafkaService)
    {
        _cacheService = cacheService;
        _kafkaService = kafkaService;
    }

    public async Task<bool> Handle(CreateAllocationRequestCommand request, CancellationToken cancellationToken)
    {
        // Redis üzerinden fabrikanın kalan kotasını hızlıca kontrol et ve düş
        string cacheKey = $"quota:{request.FactoryId}";
        bool isQuotaAvailable = await _cacheService.DecreaseQuotaAsync(cacheKey, request.RequestedTons);

        if (!isQuotaAvailable)
        {
            return false;
        }

        // Talebi asenkron işlenmek üzere Kafka kuyruğuna fırlat
        await _kafkaService.PublishAllocationRequestAsync("allocation-requests-topic", request);

        return true;
    }
}