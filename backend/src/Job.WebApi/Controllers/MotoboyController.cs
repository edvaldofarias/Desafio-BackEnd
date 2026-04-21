using Job.Application.Commands.Motoboy;
using Job.WebApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[Route("entregadores")]
[AllowAnonymous]
public sealed class MotoboyController(IMediator mediator) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMotoboyCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedMensagemActionResult();
    }

    [HttpPost("{id}/cnh")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCnh([FromRoute] string id, [FromBody] UploadCnhMotoboyCommand command, CancellationToken cancellationToken)
    {
        command.Identifier = id;
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedMensagemActionResult();
    }
}