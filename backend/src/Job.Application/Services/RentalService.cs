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
    IRequestHandler<CreateRentalCommand, Result<RentalDto>>,
    IRequestHandler<GetRentalCommand, Result<RentalDto>>
{
    public async Task<Result<RentalDto>> Handle(GetRentalCommand request, CancellationToken cancellationToken)
    {
        var rent = await rentalRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (rent is null)
            return Result.Fail("Locação não encontrada");

        var motoboy = await motoboyRepository.GetByIdAsync(rent.IdMotoboy, cancellationToken);
        var moto = await motoRepository.GetByIdAsync(rent.IdMoto, cancellationToken);
        return Result.Ok(MapToDto(rent, motoboy, moto));
    }

    public async Task<Result<RentalDto>> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var validate = await new CancelRentalValidation().ValidateAsync(request, cancellationToken);
        if (!validate.IsValid)
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));

        var rent = await rentalRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (rent is null)
            return Result.Fail("Locação não encontrada");

        rent.RegisterReturn(DateOnly.FromDateTime(request.DateReturn));
        await rentalRepository.UpdateAsync(rent, cancellationToken);

        var motoboy = await motoboyRepository.GetByIdAsync(rent.IdMotoboy, cancellationToken);
        var moto = await motoRepository.GetByIdAsync(rent.IdMoto, cancellationToken);
        return Result.Ok(MapToDto(rent, motoboy, moto)).WithSuccess("Data de devolução informada com sucesso");
    }

    public async Task<Result<RentalDto>> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        var validate = await new CreateRentalValidation().ValidateAsync(request, cancellationToken);

        var motoboy = await GetMotoboyEntity(request.MotoboyIdentifier, validate, cancellationToken);
        var moto = await GetMotoEntity(request.MotoIdentifier, validate, cancellationToken);

        var identifier = string.IsNullOrWhiteSpace(request.Identifier)
            ? Guid.NewGuid().ToString("N")
            : request.Identifier;

        if (await rentalRepository.CheckIdentifierExistsAsync(identifier, cancellationToken))
            validate.Errors.Add(new ValidationFailure("identificador", "Identificador já cadastrado"));

        if (!validate.IsValid)
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));

        var rentEntity = new RentalEntity(
            identifier,
            motoboy!.Id,
            moto!.Id,
            DateOnly.FromDateTime(request.DateStart),
            DateOnly.FromDateTime(request.DateEnd),
            DateOnly.FromDateTime(request.DatePreview),
            request.Plan);

        await rentalRepository.CreateAsync(rentEntity, cancellationToken);
        logger.LogInformation("Locação {Identifier} criada", rentEntity.Identifier);

        return Result.Ok(MapToDto(rentEntity, motoboy, moto));
    }

    private static RentalDto MapToDto(RentalEntity rent, MotoboyEntity? motoboy, MotoEntity? moto)
    {
        return new RentalDto(
            rent.Identifier,
            rent.DailyValue,
            motoboy?.Identifier ?? string.Empty,
            moto?.Identifier ?? string.Empty,
            rent.DateStart.ToDateTime(TimeOnly.MinValue),
            rent.DateEnd.ToDateTime(TimeOnly.MinValue),
            rent.DatePreview.ToDateTime(TimeOnly.MinValue),
            rent.DateReturn?.ToDateTime(TimeOnly.MinValue),
            rent.Value,
            rent.Fine);
    }

    private async Task<MotoboyEntity?> GetMotoboyEntity(string identifier, ValidationResult validate,
        CancellationToken cancellationToken)
    {
        var motoboy = await motoboyRepository.GetByIdentifierAsync(identifier, cancellationToken);
        if (motoboy is null)
        {
            validate.Errors.Add(new ValidationFailure("entregador_id", "Entregador não encontrado"));
            return null;
        }

        if (motoboy.Type is ECnhType.A or ECnhType.AB) return motoboy;

        validate.Errors.Add(new ValidationFailure("tipo_cnh",
            "Somente entregadores habilitados na categoria A podem efetuar uma locação"));
        return motoboy;
    }

    private async Task<MotoEntity?> GetMotoEntity(string identifier, ValidationResult validate,
        CancellationToken cancellationToken)
    {
        var moto = await motoRepository.GetByIdentifierAsync(identifier, cancellationToken);
        if (moto is not null) return moto;

        validate.Errors.Add(new ValidationFailure("moto_id", "Moto não encontrada"));
        return null;
    }
}