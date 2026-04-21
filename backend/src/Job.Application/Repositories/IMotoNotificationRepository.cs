using Job.Domain.Entities.Notification;

namespace Job.Application.Repositories;

public interface IMotoNotificationRepository
{
    Task CreateAsync(MotoNotificationEntity notification, CancellationToken cancellationToken);
    Task<IEnumerable<MotoNotificationEntity>> GetAllAsync(CancellationToken cancellationToken);
}
