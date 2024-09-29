namespace Job.Application.Commands.Moto;

public sealed record CreateMotoCommand(int Year, string Model, string Plate) : IRequest<Result<Guid>>;