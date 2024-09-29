using FluentResults.Extensions.AspNetCore;
using Job.Application.Commands.Rental;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[Authorize(Roles = "motoboy")]
public sealed class RentalController(
    ILogger<RentalController> logger,
    IMediator mediator
) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateRentalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando aluguel");
        var cnpj = GetCnpj();
        if (cnpj is null)
            return Unauthorized();
        command.Cnpj = cnpj;

        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel([FromBody] CancelRentalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelando aluguel");
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }
}