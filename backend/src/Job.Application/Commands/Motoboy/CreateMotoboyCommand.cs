using Job.Domain.Enums;

namespace Job.Application.Commands.Motoboy;

public sealed record CreateMotoboyCommand(
    string Name,
    string Password,
    string Cnpj,
    DateTime DateBirth,
    string Cnh,
    ECnhType TypeCnh) : IRequest<Result>;