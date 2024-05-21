using Job.Domain.Queries.Moto;
using Job.Domain.Repositories;
using Job.Domain.UseCases.Moto.GetAll.Queries;
using Job.Domain.UseCases.Moto.GetAll.Responses;
using MediatR;

namespace Job.Domain.UseCases.Moto.GetAll;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IMotoRepository motoRepository) 
    : IRequestHandler<GetAllMotoQuery, IEnumerable<MotoResponse>>
{
    public async Task<IEnumerable<MotoResponse>> Handle(GetAllMotoQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de todas as motos");
        var motos = await motoRepository.GetAllAsync(request.Quantity, request.Page, cancellationToken);
        logger.LogInformation("Busca de todas as motos finalizada com sucesso");
        return motos.Select(moto => new MotoResponse(moto.Id, moto.Year, moto.Model, moto.Plate));
    }
}
