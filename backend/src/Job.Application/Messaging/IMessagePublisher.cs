namespace Job.Application.Messaging;

public interface IMessagePublisher
{
    Task PublishMotoCreatedAsync(MotoCreatedEvent @event, CancellationToken cancellationToken);
}
