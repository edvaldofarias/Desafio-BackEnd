using Job.Domain.Commands;
using Job.Domain.Queries.Moto;
using Job.Domain.Repositories;
using Job.Domain.Services.Interfaces;

namespace Job.Domain.Services;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IMotoRepository motoRepository) : IMotoService
{

    public async Task<IEnumerable<MotoQuery>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de todas as motos");
        var motos = await motoRepository.GetAllAsync(cancellationToken);
        logger.LogInformation("Busca de todas as motos finalizada com sucesso");
        return motos.Select(moto => new MotoQuery(moto.Id, moto.Year, moto.Model, moto.Plate));
    }

    public async Task<MotoQuery?> GetByIdAsync(Guid idMoto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de uma moto");
        var moto = await motoRepository.GetByIdAsync(idMoto, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {id}", idMoto);
            return null;
        }

        logger.LogInformation("Busca de uma moto finalizada com sucesso");
        return new MotoQuery(moto.Id, moto.Year, moto.Model, moto.Plate);
    }

    public async Task<MotoQuery?> GetByPlateAsync(string placa, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de uma moto pela placa");
        var moto = await motoRepository.GetByPlateAsync(placa, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {placa}", placa);
            return null;
        }

        logger.LogInformation("Busca de uma moto pela placa finalizada com sucesso");
        return new MotoQuery(moto.Id, moto.Year, moto.Model, moto.Plate);
    }
}