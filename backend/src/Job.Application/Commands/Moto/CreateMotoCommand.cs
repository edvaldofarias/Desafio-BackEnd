using System.Text.Json.Serialization;

namespace Job.Application.Commands.Moto;

public sealed record CreateMotoCommand(
    [property: JsonPropertyName("ano")] int Year,
    [property: JsonPropertyName("modelo")] string Model,
    [property: JsonPropertyName("placa")] string Plate,
    [property: JsonPropertyName("identificador")] string? Identifier = null) : IRequest<Result>;