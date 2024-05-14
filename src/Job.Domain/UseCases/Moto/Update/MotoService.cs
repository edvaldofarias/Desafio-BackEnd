using Job.Domain.Commands;
using Job.Domain.Repositories;
using Job.Domain.UseCases.Moto.Update.Commands;
using MediatR;

namespace Job.Domain.UseCases.Moto.Update;

public sealed class MotoService(
    ILogger<MotoService> logger,
    IValidator<UpdateMotoCommand> validator,
    IMotoRepository motoRepository) : IRequestHandler<UpdateMotoCommand, CommandResponse<string>>
{
    public async Task<CommandResponse<string>> Handle(UpdateMotoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de atualização de uma moto");
        var validate = await validator.ValidateAsync(request, cancellationToken);

        var moto = await motoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (moto is null)
        {
            logger.LogInformation("Moto não encontrada {id}", request.Id);
            validate.Errors.Add(new ValidationFailure("Id", "Moto não encontrada"));
        }

        if (validate.IsValid)
        {
            moto!.Update(request.Year, request.Model, request.Plate);
            logger.LogInformation("Moto atualizada com sucesso");
            await motoRepository.UpdateAsync(moto, cancellationToken);
            return new CommandResponse<string>(moto.Id);
        }

        logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
        return new CommandResponse<string>(validate.Errors);
    }
}