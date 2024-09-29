using FluentResults.Extensions.AspNetCore;
using Job.Application.Commands.Motoboy;
using Job.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

public class MotoboyController(
    ILogger<MotoboyController> logger,
    IMediator mediator) : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authentication(AuthenticationMotoboyCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciado autenticação de motoboy");
        var response = await mediator.Send(command, cancellationToken);

        if (response.IsFailed)
            return response.ToActionResult();

        if (response.ValueOrDefault is null)
            return Unauthorized();

        const string role = "motoboy";
        var cnpj = response.Value.Cnpj;
        var token = TokenService.GenerateToken(cnpj, role);

        return Ok(new
        {
            token,
            Data = response.Value
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateMotoboyCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciado criação de motoboy");
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize (Roles = "motoboy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage([FromForm] UploadCnhMotoboyCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciado upload de imagem");
        var cnpj = GetCnpj();
        command.Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }
}