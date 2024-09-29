using Job.Domain.Entities.User;

namespace Job.Application.Repositories;

public interface IManagerRepository
{
    Task<ManagerEntity?> GetAsync(string email, string password, CancellationToken cancellationToken = default);
}