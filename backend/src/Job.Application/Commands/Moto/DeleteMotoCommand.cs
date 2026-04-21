namespace Job.Application.Commands.Moto;

public sealed record DeleteMotoCommand(string Identifier) : IRequest<Result>;