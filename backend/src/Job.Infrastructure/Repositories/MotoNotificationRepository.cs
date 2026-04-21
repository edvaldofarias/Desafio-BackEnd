using Job.Application.Repositories;
using Job.Domain.Entities.Notification;

namespace Job.Infrastructure.Repositories;

public sealed class MotoNotificationRepository(JobContext context) : IMotoNotificationRepository
{
    public async Task CreateAsync(MotoNotificationEntity notification, CancellationToken cancellationToken)
    {
        await context.MotoNotifications.AddAsync(notification, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MotoNotificationEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.MotoNotifications.AsNoTracking().ToListAsync(cancellationToken);
    }
}
