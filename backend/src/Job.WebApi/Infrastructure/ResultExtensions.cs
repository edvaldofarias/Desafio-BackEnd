using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Infrastructure;

public static class ResultExtensions
{
    public static IActionResult ToMensagemActionResult(this Result result, int successStatus = StatusCodes.Status200OK, string? successMessage = null)
    {
        if (result.IsFailed)
            return BuildFailure(result.Errors.Select(e => e.Message));

        var message = successMessage
            ?? result.Successes.LastOrDefault()?.Message;

        if (string.IsNullOrEmpty(message))
            return new StatusCodeResult(successStatus);

        return new ObjectResult(new { mensagem = message }) { StatusCode = successStatus };
    }

    public static IActionResult ToMensagemActionResult<T>(this Result<T> result, int successStatus = StatusCodes.Status200OK)
    {
        if (result.IsFailed)
            return BuildFailure(result.Errors.Select(e => e.Message));

        return new ObjectResult(result.Value) { StatusCode = successStatus };
    }

    public static IActionResult ToCreatedMensagemActionResult(this Result result)
    {
        if (result.IsFailed)
            return BuildFailure(result.Errors.Select(e => e.Message));

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    private static IActionResult BuildFailure(IEnumerable<string> errors)
    {
        var first = errors.FirstOrDefault() ?? "Dados inválidos";
        var status = first.Contains("não encontrad", StringComparison.OrdinalIgnoreCase)
            ? StatusCodes.Status404NotFound
            : StatusCodes.Status400BadRequest;
        return new ObjectResult(new { mensagem = first }) { StatusCode = status };
    }
}
