using Job.Domain.Commands;
using Job.Domain.Commands.User.Manager;
using Job.Domain.Dtos.User;

namespace Job.Domain.Services.Interfaces;

public interface IManagerService
{
    Task<CommandResponse<ManagerDto?>> GetManager(AuthenticationManagerCommand command, CancellationToken cancellationToken = default);
}