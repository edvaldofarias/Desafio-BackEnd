using Job.Domain.Commands;
using Job.Domain.Commands.Rent;
using Job.Domain.Commands.Rent.Validations;
using Job.Domain.Dtos.Rental;
using Job.Domain.Entities.Moto;
using Job.Domain.Entities.Rental;
using Job.Domain.Entities.User;
using Job.Domain.Enums;
using Job.Domain.Repositories;
using Job.Domain.Services.Interfaces;

namespace Job.Domain.Services;

public sealed class RentalService(
    ILogger<RentalService> logger,
    IRentalRepository rentalRepository,
    IMotoRepository motoRepository,
    IMotoboyRepository motoboyRepository
) : IRentService
{
    public async Task<CommandResponse<RentalDto>> CreateRentAsync(CreateRentCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de criação de um aluguel");
        var validate = await new CreateRentalValidation().ValidateAsync(command, cancellationToken);

        logger.LogInformation("Buscando motoboy");
        var motoboy = await GetMotoboyEntity(command, validate, cancellationToken);

        var moto = await GetMotoEntity(command, cancellationToken, validate);

        if (!validate.IsValid)
        {
            logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
            return new CommandResponse<RentalDto>(validate.Errors);
        }

        logger.LogInformation("Criando objeto aluguel");
        var rentEntity = new RentalEntity(motoboy!.Id, moto!.Id, DateOnly.FromDateTime(command.DatePreview),
            command.Plan);

        await rentalRepository.CreateAsync(rentEntity, cancellationToken);

        logger.LogInformation("Aluguel criado com sucesso");
        return new CommandResponse<RentalDto>
        {
            Data = new RentalDto(rentEntity.Id, rentEntity.Value, rentEntity.Plan)
        };
    }

    public async Task<CommandResponse<RentalDto>> CancelRentAsync(CancelRentCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de cancelamento de um aluguel");
        var validate = await new CancelRentalValidation().ValidateAsync(command, cancellationToken);

        logger.LogInformation("Buscando aluguel");
        var rent = await rentalRepository.GetByIdAsync(command.Id, cancellationToken);
        if (rent is null)
        {
            logger.LogInformation("Aluguel não encontrado {id}", command.Id);
            validate.Errors.Add(new ValidationFailure("Id", "Aluguel não encontrado"));
        }

        if (!validate.IsValid)
        {
            logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
            return new CommandResponse<RentalDto>(validate.Errors);
        }

        var fine = rent!.CalculateFine(DateOnly.FromDateTime(command.DatePreview));
        await rentalRepository.UpdateAsync(rent, cancellationToken);

        return new CommandResponse<RentalDto>
        {
            Data = new RentalDto(rent.Id, rent.Value, rent.Plan, fine)
        };
    }

    #region private methods

    private async Task<MotoboyEntity?> GetMotoboyEntity(CreateRentCommand command, ValidationResult validate,
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

    private async Task<MotoEntity?> GetMotoEntity(CreateRentCommand command, CancellationToken cancellationToken, ValidationResult validate)
    {
        var moto = await motoRepository.GetByIdAsync(command.IdMoto, cancellationToken);

        if (moto is not null) return moto;

        logger.LogInformation("Moto não encontrada {id}", command.IdMoto);
        validate.Errors.Add(new ValidationFailure("IdMoto", "Moto não encontrada"));

        return moto;
    }

    #endregion
}