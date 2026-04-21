using Job.Domain.Entities.Moto;

namespace Job.Infrastructure.Repositories;

public class MotoRepository(JobContext context) : IMotoRepository
{
    public async Task CreateAsync(MotoEntity moto, CancellationToken cancellationToken)
    {
        await context.Motos.AddAsync(moto, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MotoEntity moto, CancellationToken cancellationToken)
    {
        context.Motos.Update(moto);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MotoEntity moto, CancellationToken cancellationToken)
    {
        context.Motos.Remove(moto);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<MotoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Motos.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MotoEntity?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        return await context.Motos.Where(x => x.Identifier == identifier).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MotoEntity?> GetByPlateAsync(string plate, CancellationToken cancellationToken)
    {
        return await context.Motos.Where(x => x.Plate == plate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<MotoEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Motos.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MotoEntity>> SearchAsync(string? plate, CancellationToken cancellationToken)
    {
        var query = context.Motos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(plate))
        {
            var normalized = plate.Replace("-", string.Empty);
            query = query.Where(x => x.Plate == normalized);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckPlateExistsAsync(string plate, CancellationToken cancellationToken)
    {
        return await context.Motos.AnyAsync(x => x.Plate == plate, cancellationToken);
    }

    public async Task<bool> CheckIdentifierExistsAsync(string identifier, CancellationToken cancellationToken)
    {
        return await context.Motos.AnyAsync(x => x.Identifier == identifier, cancellationToken);
    }
}