using Job.Application.Dtos.Moto;

namespace Job.Application.Commands.Moto;

public sealed record GetAllMotoCommand(string? Plate = null) : IRequest<Result<IEnumerable<MotoDto>>>;