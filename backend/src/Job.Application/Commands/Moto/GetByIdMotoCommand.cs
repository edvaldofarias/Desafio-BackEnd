using Job.Application.Dtos.Moto;

namespace Job.Application.Commands.Moto;

public sealed record GetByIdMotoCommand(string Identifier) : IRequest<Result<MotoDto>>;