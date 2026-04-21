namespace Job.Application.Commands.Moto;

public sealed record UpdateMotoCommand(Guid Id, string Plate) : IRequest<Result>;