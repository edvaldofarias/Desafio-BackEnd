using System.Text.Json.Serialization;

namespace Job.Application.Commands.Moto;

public sealed record UpdateMotoCommand(
    [property: JsonIgnore] string Identifier,
    [property: JsonPropertyName("placa")] string Plate) : IRequest<Result>;