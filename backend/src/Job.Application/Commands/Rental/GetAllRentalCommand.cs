namespace Job.Application.Commands.Rental;

public sealed record GetAllRentalCommand(string MotoboyIdentifier) : IRequest<Result<IEnumerable<Job.Application.Dtos.Rental.RentalDto>>>;