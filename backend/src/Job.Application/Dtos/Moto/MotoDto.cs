using System.Text.Json.Serialization;

namespace Job.Application.Dtos.Moto;

public sealed record MotoDto(
    [property: JsonPropertyName("identificador")] string Identifier,
    [property: JsonPropertyName("ano")] int Year,
    [property: JsonPropertyName("modelo")] string Model,
    [property: JsonPropertyName("placa")] string Plate);