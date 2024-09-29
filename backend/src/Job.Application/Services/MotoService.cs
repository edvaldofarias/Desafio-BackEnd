using FluentValidation.Results;
using Job.Application.Commands.Moto;
using Job.Application.Commands.Moto.Validations;
using Job.Application.Dtos.Moto;
using Job.Application.Repositories;
using Job.Domain.Entities.Moto;

namespace Job.Application.Services;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IMotoRepository motoRepository,
    IRentalRepository rentalRepository) :
    IRequestHandler<CreateMotoCommand, Result<Guid>>,
    IRequestHandler<UpdateMotoCommand, Result>,
    IRequestHandler<GetByIdMotoCommand, Result<MotoDto>>,
    IRequestHandler<GetAllMotoCommand, Result<IEnumerable<MotoDto>>>,
    IRequestHandler<GetByPlateMotoCommand, Result<MotoDto>>,
    IRequestHandler<DeleteMotoCommand, Result>
{
    public async Task<Result<Guid>> Handle(CreateMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de criação de uma moto");
        var validator = await new CreateMotoValidation().ValidateAsync(request, cancellationToken);

        var moto = new MotoEntity(request.Year, request.Model, request.Plate);
        logger.LogInformation("Objeto moto criado com sucesso");
        if (validator.IsValid && await motoRepository.CheckPlateExistsAsync(moto.Plate, cancellationToken))
        {
            logger.LogInformation("Placa já cadastrada {plate}", moto.Plate);
            validator.Errors.Add(new ValidationFailure("Plate", "Placa já cadastrada"));
        }

        if (validator.IsValid)
        {
            logger.LogInformation("Moto criada com sucesso");
            await motoRepository.CreateAsync(moto, cancellationToken);
            return Result.Ok(moto.Id);
        }

        logger.LogInformation("Erros de validação encontrados {errors}", validator.Errors);
        return Result.Fail(validator.Errors.Select(x => x.ErrorMessage));
    }

    public async Task<Result> Handle(UpdateMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de atualização de uma moto");
        var validator = await new UpdateMotoValidation().ValidateAsync(request, cancellationToken);

        var moto = await motoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {id}", request.Id);
            validator.Errors.Add(new ValidationFailure("Id", "Moto não encontrada"));
        }

        if (validator.IsValid)
        {
            moto!.Update(request.Year, request.Model, request.Plate);
            logger.LogInformation("Moto atualizada com sucesso");
            await motoRepository.UpdateAsync(moto, cancellationToken);
            return Result.Ok();
        }

        logger.LogInformation("Erros de validação encontrados {errors}", validator.Errors);
        return Result.Fail(validator.Errors.Select(x => x.ErrorMessage));
    }

    public async Task<Result<MotoDto>> Handle(GetByIdMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de uma moto");
        var moto = await motoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {id}", request.Id);
            return Result.Ok();
        }

        logger.LogInformation("Busca de uma moto finalizada com sucesso");
        var motoDto =  new MotoDto(moto.Id, moto.Year, moto.Model, moto.Plate);
        return Result.Ok(motoDto);
    }

    public async Task<Result<IEnumerable<MotoDto>>> Handle(GetAllMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de todas as motos");
        var motos = await motoRepository.GetAllAsync(cancellationToken);
        logger.LogInformation("Busca de todas as motos finalizada com sucesso");
        var motoDtos = motos.Select(moto => new MotoDto(moto.Id, moto.Year, moto.Model, moto.Plate));
        return Result.Ok(motoDtos);
    }

    public async Task<Result<MotoDto>> Handle(GetByPlateMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a busca de uma moto pela placa");
        var moto = await motoRepository.GetByPlateAsync(request.plate, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {placa}", request.plate);
            return Result.Ok();
        }

        logger.LogInformation("Busca de uma moto pela placa finalizada com sucesso");
        var motoDto = new MotoDto(moto.Id, moto.Year, moto.Model, moto.Plate);
        return Result.Ok(motoDto);
    }

    public async Task<Result> Handle(DeleteMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de exclusão de uma moto");
        var moto = await motoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {id}", request.Id);
            return Result.Fail("Moto não encontrada");
        }

        var rent = await rentalRepository.GetByMotoIdAsync(moto.Id, cancellationToken);

        if(rent is not null && rent.DateEnd > DateOnly.FromDateTime(DateTime.Now))
        {
            logger.LogInformation("Moto com aluguel ativo");
            return Result.Fail("Moto com aluguel ativo");
        }

        logger.LogInformation("Moto excluída com sucesso");
        await motoRepository.DeleteAsync(moto, cancellationToken);
        return Result.Ok();
    }
}