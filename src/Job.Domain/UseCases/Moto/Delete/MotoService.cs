using Job.Domain.Commands;
using Job.Domain.Entities.Moto;
using Job.Domain.Repositories;
using Job.Domain.UseCases.Moto.Delete.Commands;
using MediatR;

namespace Job.Domain.UseCases.Moto.Delete;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IValidator<DeleteMotoCommand> validator,
    IMotoRepository motoRepository,
    IRentalRepository rentalRepository
) : IRequestHandler<DeleteMotoCommand, CommandResponse<string>>
{
    public async Task<CommandResponse<string>> Handle(DeleteMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de exclusão de uma moto");
        var validate = await validator.ValidateAsync(request, cancellationToken);

        if (validate.IsValid is false) return new CommandResponse<string>(validate.Errors);

        var moto = await motoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {id}", request.Id);
            validate.Errors.Add(new ValidationFailure("Id", "Moto não encontrada"));
        }

        await IsRentalActiveAsync(request.Id, validate, cancellationToken);

        if (validate.IsValid is false) return new CommandResponse<string>(validate.Errors);

        await motoRepository.DeleteAsync(moto!, cancellationToken);
        logger.LogInformation("Moto excluída com sucesso - {Id}", moto!.Id);
        return new CommandResponse<string>(moto.Id);
    }

    private async Task IsRentalActiveAsync(Guid idMoto, ValidationResult validate, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetByMotoIdAsync(idMoto, cancellationToken);
        if (rental is not null && rental.DateEnd > DateOnly.FromDateTime(DateTime.Now))
        {
            logger.LogInformation("Moto com aluguel ativo");
            validate.Errors.Add(new ValidationFailure("Rental", "Moto com aluguel ativo"));
        }
    }
}