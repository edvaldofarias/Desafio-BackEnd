using FluentResults.Extensions.AspNetCore;
using Job.Application.Commands.Moto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

public class MotoController(
    ILogger<MotoController> logger,
    IMediator mediator) : BaseController
{
    [HttpGet]
    [Authorize(Roles = "admin,motoboy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Recuperando todos as motos cadastradas");
        var result = await mediator.Send(new GetAllMotoCommand(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Roles = "admin,motoboy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Recuperando moto por id {id}", id);
        var result = await mediator.Send(new GetByIdMotoCommand(id), cancellationToken);
        return result.ValueOrDefault is null ? NotFound() : result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Roles = "admin,motoboy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPlate([FromQuery] string plate, CancellationToken cancellationToken)
    {
        logger.LogInformation("Recuperando moto por placa {plate}", plate);
        var result = await mediator.Send(new GetByPlateMotoCommand(plate), cancellationToken);
        return result.ValueOrDefault is null ? NotFound() : result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateMotoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando moto");
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPut]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateMotoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Atualizando moto");
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpDelete]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deletando moto");
        var response = await mediator.Send(new DeleteMotoCommand(id), cancellationToken);
        return response.ToActionResult();
    }
}