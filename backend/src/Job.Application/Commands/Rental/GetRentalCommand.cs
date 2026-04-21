using Job.Application.Dtos.Rental;

namespace Job.Application.Commands.Rental;

public sealed record GetRentalCommand(string Identifier) : IRequest<Result<RentalDto>>;
