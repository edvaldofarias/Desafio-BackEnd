namespace Job.Application.Commands.Moto;

public sealed record UpdateMotoCommand(Guid Id, int Year, string Model, string Plate) : IRequest<Result>;