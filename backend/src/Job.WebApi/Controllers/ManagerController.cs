using FluentResults.Extensions.AspNetCore;
using Job.Application.Commands.Manager;
using Job.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[AllowAnonymous]
public class ManagerController(
    ILogger<ManagerController> logger,
    IMediator mediator) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Authentication(AuthenticationManagerCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciado autenticação de admin");
        var response = await mediator.Send(command, cancellationToken);

        if (!response.IsSuccess) return response.ToActionResult();

        if (response.ValueOrDefault is null) return Unauthorized();

        var query = response.Value;
        var token = TokenService.GenerateToken(query.Email, "admin");
        return Ok(new
        {
            token,
            Data = query
        });
    }
}