using Job.Domain.Commands;
using Job.Domain.Queries.Moto;

namespace Job.Domain.Services.Interfaces;

public interface IMotoService
{
    Task<IEnumerable<MotoQuery>> GetAllAsync(CancellationToken cancellationToken);
    Task<MotoQuery?> GetByIdAsync(Guid idMoto, CancellationToken cancellationToken);
    Task<MotoQuery?> GetByPlateAsync(string placa, CancellationToken cancellationToken);
}