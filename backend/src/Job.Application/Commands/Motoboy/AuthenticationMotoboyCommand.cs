using Job.Application.Dtos.Motoboy;

namespace Job.Application.Commands.Motoboy;

public sealed record AuthenticationMotoboyCommand(string Cnpj, string Password) : IRequest<Result<MotoboyDto>>;