using FluentValidation.Results;
using Job.Application.Commands.Moto;
using Job.Application.Commands.Moto.Validations;
using Job.Application.Dtos.Moto;
using Job.Application.Messaging;
using Job.Application.Repositories;
using Job.Domain.Entities.Moto;

namespace Job.Application.Services;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IMotoRepository motoRepository,
    IRentalRepository rentalRepository,
    IMessagePublisher messagePublisher) :
    IRequestHandler<CreateMotoCommand, Result>,
    IRequestHandler<UpdateMotoCommand, Result>,
    IRequestHandler<GetByIdMotoCommand, Result<MotoDto>>,
    IRequestHandler<GetAllMotoCommand, Result<IEnumerable<MotoDto>>>,
    IRequestHandler<DeleteMotoCommand, Result>
{
    public async Task<Result> Handle(CreateMotoCommand request, CancellationToken cancellationToken)
    {
        var validator = await new CreateMotoValidation().ValidateAsync(request, cancellationToken);

        if (!validator.IsValid)
            return Result.Fail(validator.Errors.Select(x => x.ErrorMessage));

        var moto = new MotoEntity(request.Identifier, request.Year, request.Model, request.Plate);

        if (await motoRepository.CheckIdentifierExistsAsync(moto.Identifier, cancellationToken))
            return Result.Fail("Identificador já cadastrado");

        if (await motoRepository.CheckPlateExistsAsync(moto.Plate, cancellationToken))
            return Result.Fail("Placa já cadastrada");

        await motoRepository.CreateAsync(moto, cancellationToken);

        var @event = new MotoCreatedEvent(moto.Id, moto.Year, moto.Model, moto.Plate, DateTime.UtcNow);
        await messagePublisher.PublishMotoCreatedAsync(@event, cancellationToken);

        logger.LogInformation("Moto {Identifier} criada com sucesso", moto.Identifier);
        return Result.Ok();
    }

    public async Task<Result> Handle(UpdateMotoCommand request, CancellationToken cancellationToken)
    {
        var validator = await new UpdateMotoValidation().ValidateAsync(request, cancellationToken);

        var moto = await motoRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (moto is null)
            return Result.Fail("Moto não encontrada");

        if (!validator.IsValid)
            return Result.Fail(validator.Errors.Select(x => x.ErrorMessage));

        var newPlate = request.Plate.Replace("-", string.Empty);
        if (newPlate != moto.Plate && await motoRepository.CheckPlateExistsAsync(newPlate, cancellationToken))
            return Result.Fail("Placa já cadastrada");

        moto.UpdatePlate(request.Plate);
        await motoRepository.UpdateAsync(moto, cancellationToken);
        return Result.Ok().WithSuccess("Placa modificada com sucesso");
    }

    public async Task<Result<MotoDto>> Handle(GetByIdMotoCommand request, CancellationToken cancellationToken)
    {
        var moto = await motoRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (moto is null)
            return Result.Fail("Moto não encontrada");

        return Result.Ok(new MotoDto(moto.Identifier, moto.Year, moto.Model, moto.Plate));
    }

    public async Task<Result<IEnumerable<MotoDto>>> Handle(GetAllMotoCommand request, CancellationToken cancellationToken)
    {
        var motos = await motoRepository.SearchAsync(request.Plate, cancellationToken);
        var motoDtos = motos.Select(moto => new MotoDto(moto.Identifier, moto.Year, moto.Model, moto.Plate));
        return Result.Ok(motoDtos);
    }

    public async Task<Result> Handle(DeleteMotoCommand request, CancellationToken cancellationToken)
    {
        var moto = await motoRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (moto is null)
            return Result.Fail("Moto não encontrada");

        if (await rentalRepository.ExistsForMotoAsync(moto.Id, cancellationToken))
            return Result.Fail("Moto possui registro de locações e não pode ser removida");

        await motoRepository.DeleteAsync(moto, cancellationToken);
        return Result.Ok();
    }
}