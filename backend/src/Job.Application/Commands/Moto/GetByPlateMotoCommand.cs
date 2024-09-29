using Job.Application.Dtos.Moto;

namespace Job.Application.Commands.Moto;

public sealed record GetByPlateMotoCommand(string plate) : IRequest<Result<MotoDto>>;