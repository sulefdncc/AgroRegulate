namespace AgroRegulate.Application.Interfaces;

public interface IKafkaProducerService
{
    Task PublishAllocationRequestAsync<T>(string topic, T message);
}