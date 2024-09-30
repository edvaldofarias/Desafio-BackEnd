using Job.Application.Commands.Manager;
using Job.Application.Dtos.Manager;
using Job.Application.Repositories;

namespace Job.Application.Services;

public sealed class ManagerService(
    ILogger<ManagerService> logger,
    IManagerRepository managerRepository,
    IValidator<AuthenticationManagerCommand> validator
) : IRequestHandler<AuthenticationManagerCommand, Result<ManagerDto>>
{
    private const int WorkFactor = 12;

    public async Task<Result<ManagerDto>> Handle(AuthenticationManagerCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando admin {email}", request.Email);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid is false)
        {
            logger.LogError("Comando inválido");
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));
        }

        var password = BCrypt.Net.BCrypt.HashPassword(request.Password, WorkFactor);
        var manager = await managerRepository.GetAsync(request.Email, password, cancellationToken);
        if (manager is null)
        {
            logger.LogError("Manager not found");
            return Result.Ok();
        }

        logger.LogInformation("Admin encontrado com sucesso");
        var query = new ManagerDto(manager.Id, manager.Email);
        return Result.Ok(query);
    }
}