using Job.Application.Commands.Moto;
using Job.WebApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[Route("motos")]
[Authorize(Roles = "admin")]
public sealed class MotoController(IMediator mediator) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMotoCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedMensagemActionResult();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery(Name = "placa")] string? placa, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllMotoCommand(placa), cancellationToken);
        return result.ToMensagemActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdMotoCommand(id), cancellationToken);
        return result.ToMensagemActionResult();
    }

    [HttpPut("{id}/placa")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePlate([FromRoute] string id, [FromBody] UpdatePlateRequest body, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMotoCommand(id, body.Placa), cancellationToken);
        return result.ToMensagemActionResult(successMessage: "Placa modificada com sucesso");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteMotoCommand(id), cancellationToken);
        return result.ToMensagemActionResult();
    }

    public sealed record UpdatePlateRequest(string Placa);
}