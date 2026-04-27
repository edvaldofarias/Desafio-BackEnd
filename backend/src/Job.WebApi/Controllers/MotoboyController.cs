using Job.Application.Commands.Motoboy;
using Job.WebApi.Infrastructure;
using Job.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[Route("entregadores")]
[Authorize(Roles = "entregador")]
public sealed class MotoboyController(IMediator mediator, ITokenService tokenService) : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMotoboyCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedMensagemActionResult();
    }

    [HttpPost("authentication")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authentication([FromBody] AuthenticationMotoboyCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailed)
            return result.ToMensagemActionResult();
        if (result.ValueOrDefault is null)
            return Unauthorized(new { mensagem = "Credenciais inválidas" });

        var token = tokenService.GenerateToken(result.Value.Cnpj, "entregador");
        return Ok(new { token, data = result.Value });
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