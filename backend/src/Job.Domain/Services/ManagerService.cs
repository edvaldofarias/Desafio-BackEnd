using Job.Domain.Commands;
using Job.Domain.Commands.User.Manager;
using Job.Domain.Commons;
using Job.Domain.Dtos.User;
using Job.Domain.Repositories;
using Job.Domain.Services.Interfaces;

namespace Job.Domain.Services;

public sealed class ManagerService(
    ILogger<ManagerService> logger,
    IManagerRepository managerRepository,
    IValidator<AuthenticationManagerCommand> validator) : IManagerService
{
    public async Task<CommandResponse<ManagerDto?>> GetManager(AuthenticationManagerCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Buscando admin {email}", command.Email);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if(validationResult.IsValid is false)
        {
            logger.LogError("Comando inválido");
            return new CommandResponse<ManagerDto?>(validationResult.Errors);
        }

        var manager = await managerRepository.GetAsync(command.Email, Cryptography.Encrypt(command.Password), cancellationToken);
        if (manager is null)
        {
            logger.LogError("Manager not found");
            return new CommandResponse<ManagerDto?>();
        }

        logger.LogInformation("Admin encontrado com sucesso");
        var query = new ManagerDto(manager.Id, manager.Email);
        return new CommandResponse<ManagerDto?>(query.Id)
        {
            Data = query
        };
    }
}