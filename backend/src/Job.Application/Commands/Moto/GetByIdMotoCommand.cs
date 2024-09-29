using Job.Application.Dtos.Moto;

namespace Job.Application.Commands.Moto;

public sealed record GetByIdMotoCommand(Guid Id) : IRequest<Result<MotoDto>>;