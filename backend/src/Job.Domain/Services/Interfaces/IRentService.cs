using Job.Domain.Commands;
using Job.Domain.Commands.Rent;

namespace Job.Domain.Services.Interfaces;

public interface IRentService
{
    Task<CommandResponse<string>> CreateRentAsync(CreateRentalCommand command, CancellationToken cancellationToken);

    Task<CommandResponse<string>> CancelRentAsync(CancelRentalCommand command, CancellationToken cancellationToken);
}