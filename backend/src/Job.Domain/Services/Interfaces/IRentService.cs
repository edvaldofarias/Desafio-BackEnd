using Job.Domain.Commands;
using Job.Domain.Commands.Rent;
using Job.Domain.Dtos.Rental;

namespace Job.Domain.Services.Interfaces;

public interface IRentService
{
    Task<CommandResponse<RentalDto>> CreateRentAsync(CreateRentCommand command, CancellationToken cancellationToken);

    Task<CommandResponse<RentalDto>> CancelRentAsync(CancelRentCommand command, CancellationToken cancellationToken);
}