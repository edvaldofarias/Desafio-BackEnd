using System.Text.Json.Serialization;
using Job.Domain.Enums;

namespace Job.Application.Commands.Motoboy;

public sealed record CreateMotoboyCommand(
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("cnpj")] string Cnpj,
    [property: JsonPropertyName("senha")] string Password,
    [property: JsonPropertyName("data_nascimento")] DateTime DateBirth,
    [property: JsonPropertyName("numero_cnh")] string Cnh,
    [property: JsonPropertyName("tipo_cnh")] string TypeCnh,
    [property: JsonPropertyName("identificador")] string? Identifier = null,
    [property: JsonPropertyName("imagem_cnh")] string? CnhImage = null) : IRequest<Result>;