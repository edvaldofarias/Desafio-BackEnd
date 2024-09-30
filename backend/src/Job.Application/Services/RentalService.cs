using FluentValidation.Results;
using Job.Application.Commands.Rental;
using Job.Application.Commands.Rental.Validations;
using Job.Application.Dtos.Rental;
using Job.Application.Repositories;
using Job.Domain.Entities.Moto;
using Job.Domain.Entities.Rental;
using Job.Domain.Entities.User;
using Job.Domain.Enums;

namespace Job.Application.Services;

public sealed class RentalService(
    ILogger<RentalService> logger,
    IRentalRepository rentalRepository,
    IMotoboyRepository motoboyRepository,
    IMotoRepository motoRepository) :
    IRequestHandler<CancelRentalCommand, Result<RentalDto>>,
    IRequestHandler<CreateRentalCommand, Result<RentalDto>>
{
    public async Task<Result<RentalDto>> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de cancelamento de um aluguel");
        var validate = await new CancelRentalValidation().ValidateAsync(request, cancellationToken);

        logger.LogInformation("Buscando aluguel");
        var rent = await rentalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rent is null)
        {
            logger.LogInformation("Aluguel não encontrado {id}", request.Id);
            validate.Errors.Add(new ValidationFailure("Id", "Aluguel não encontrado"));
        }

        if (!validate.IsValid)
        {
            logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));
        }

        rent!.CalculateFine(DateOnly.FromDateTime(request.DatePreview));
        await rentalRepository.UpdateAsync(rent, cancellationToken);

        var rentalDto = new RentalDto(rent.Id, rent.Value, rent.Plan, rent.Fine);
        return Result.Ok(rentalDto).WithSuccess("Cancelamento realizado com sucesso");
    }

    public async Task<Result<RentalDto>> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de criação de um aluguel");
        var validate = await new CreateRentalValidation().ValidateAsync(request, cancellationToken);

        logger.LogInformation("Buscando motoboy");
        var motoboy = await GetMotoboyEntity(request, validate, cancellationToken);

        var moto = await GetMotoEntity(request, validate, cancellationToken);

        if (!validate.IsValid)
        {
            logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));
        }

        logger.LogInformation("Criando objeto aluguel");
        var rentEntity = new RentalEntity(
            motoboy!.Id, 
            moto!.Id, DateOnly.FromDateTime(request.DatePreview),
            request.Plan);

        await rentalRepository.CreateAsync(rentEntity, cancellationToken);

        logger.LogInformation("Aluguel criado com sucesso");
        var rentalDto = new RentalDto(rentEntity.Id, rentEntity.Value, rentEntity.Plan);
        return Result.Ok(rentalDto);
    }
    
    #region private methods

    private async Task<MotoboyEntity?> GetMotoboyEntity(CreateRentalCommand command, ValidationResult validate,
        CancellationToken cancellationToken)
    {
        var motoboy = await motoboyRepository.GetByCnpjAsync(command.Cnpj, cancellationToken);

        if (motoboy is null)
        {
            logger.LogInformation("Motoboy não encontrado {Cnpj}", command.Cnpj);
            validate.Errors.Add(new ValidationFailure("IdMotoboy", "Motoboy não encontrado"));
        }

        if (motoboy?.Type != ECnhType.B) return motoboy;
        logger.LogInformation("Moto boy não possuir CNH Tipo A");
        validate.Errors.Add(new ValidationFailure("TypeCnh", "Tipo de CNH não compatível"));

        return motoboy;
    }

    private async Task<MotoEntity?> GetMotoEntity(CreateRentalCommand command, ValidationResult validate, CancellationToken cancellationToken)
    {
        var moto = await motoRepository.GetByIdAsync(command.IdMoto, cancellationToken);

        if (moto is not null) return moto;

        logger.LogInformation("Moto não encontrada {id}", command.IdMoto);
        validate.Errors.Add(new ValidationFailure("IdMoto", "Moto não encontrada"));

        return moto;
    }

    #endregion
}