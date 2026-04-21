using System.Text.Json.Serialization;
using Job.Application.Commands.Rental;
using Job.WebApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[Route("locacao")]
[AllowAnonymous]
public sealed class RentalController(IMediator mediator) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRentalCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailed)
            return result.ToMensagemActionResult();
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRentalCommand(id), cancellationToken);
        return result.ToMensagemActionResult();
    }

    [HttpPut("{id}/devolucao")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Return([FromRoute] string id, [FromBody] ReturnRentalRequest body, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelRentalCommand(id, body.DataDevolucao), cancellationToken);
        if (result.IsFailed)
            return result.ToMensagemActionResult();
        return Ok(new { mensagem = "Data de devolução informada com sucesso" });
    }

    public sealed class ReturnRentalRequest
    {
        [JsonPropertyName("data_devolucao")]
        public DateTime DataDevolucao { get; init; }
    }
}