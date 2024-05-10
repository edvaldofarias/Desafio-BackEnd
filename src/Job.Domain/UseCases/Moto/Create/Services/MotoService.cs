using Job.Domain.Commands;
using Job.Domain.Entities.Moto;
using Job.Domain.Repositories;
using Job.Domain.UseCases.Moto.Create.Commands;
using MediatR;

namespace Job.Domain.UseCases.Moto.Create.Services;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IValidator<CreateMotoCommand> validator,
    IMotoRepository motoRepository) : IRequestHandler<CreateMotoCommand, CommandResponse<string>>
{
    public async Task<CommandResponse<string>> Handle(CreateMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de criação de uma moto");
        var validate = await validator.ValidateAsync(request, cancellationToken);

        var moto = new MotoEntity(request.Year, request.Model, request.Plate);
        logger.LogInformation("Objeto moto criado com sucesso");
        if (validate.IsValid && await motoRepository.CheckPlateExistsAsync(moto.Plate, cancellationToken))
        {
            logger.LogInformation("Placa já cadastrada {plate}", moto.Plate);
            validate.Errors.Add(new ValidationFailure("Plate", "Placa já cadastrada"));
        }

        if (validate.IsValid)
        {
            logger.LogInformation("Moto criada com sucesso");
            await motoRepository.CreateAsync(moto, cancellationToken);
            return new CommandResponse<string>(moto.Id);
        }

        logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
        return new CommandResponse<string>(validate.Errors);
    }
}