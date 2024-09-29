using Job.Application.Dtos.Rental;

namespace Job.Application.Commands.Rental;

public sealed record CancelRentalCommand(Guid Id, DateTime DatePreview) : IRequest<Result<RentalDto>>;