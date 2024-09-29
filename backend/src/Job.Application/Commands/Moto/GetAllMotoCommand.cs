using Job.Application.Dtos.Moto;

namespace Job.Application.Commands.Moto;

public sealed record GetAllMotoCommand : IRequest<Result<IEnumerable<MotoDto>>>;