using Job.Domain.Entities.User;

namespace Job.Infrastructure.Repositories;

public class ManagerRepository(JobContext context) : IManagerRepository
{
    public async Task<ManagerEntity?> GetAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Managers.Where(x => x.Email == email).SingleOrDefaultAsync(cancellationToken);
    }
}