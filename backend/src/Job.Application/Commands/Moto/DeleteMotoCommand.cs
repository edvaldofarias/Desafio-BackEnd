namespace Job.Application.Commands.Moto;

public sealed record DeleteMotoCommand(Guid Id) : IRequest<Result>;