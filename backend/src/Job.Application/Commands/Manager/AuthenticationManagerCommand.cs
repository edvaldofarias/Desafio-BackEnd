using Job.Application.Dtos.Manager;

namespace Job.Application.Commands.Manager;

public sealed record AuthenticationManagerCommand(string Email, string Password) : IRequest<Result<ManagerDto>>;