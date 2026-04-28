using Job.Domain.Entities.Rental;

namespace Job.Application.Repositories;

public interface IRentalRepository
{
    Task CreateAsync(RentalEntity rental, CancellationToken cancellationToken);

    Task<RentalEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RentalEntity?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken);
    Task<IEnumerable<RentalEntity>> GetAllByMotoboyIdAsync(Guid motoboyId, CancellationToken cancellationToken);

    Task<RentalEntity?> GetByMotoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsForMotoAsync(Guid motoId, CancellationToken cancellationToken);
    Task<bool> CheckIdentifierExistsAsync(string identifier, CancellationToken cancellationToken);

    Task UpdateAsync(RentalEntity rental, CancellationToken cancellationToken);
}