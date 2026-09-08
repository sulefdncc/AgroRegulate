using System.Text.Json;
using AgroRegulate.Application.Interfaces;
using Confluent.Kafka;

namespace AgroRegulate.Infrastructure.Services;

public class KafkaProducerService : IKafkaProducerService
{
    public async Task PublishAllocationRequestAsync<T>(string topic, T message)
    {
        var jsonMessage = JsonSerializer.Serialize(message);
        Console.WriteLine($"[KAFKA PRODUCER] Topik: {topic} -> Mesaj Kuyruğa İletildi: {jsonMessage}");
        await Task.CompletedTask;
    }
}