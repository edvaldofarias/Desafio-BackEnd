using System.Text.Json.Serialization;
using Job.Application.Dtos.Rental;
using Job.Domain.Enums;

namespace Job.Application.Commands.Rental;

public sealed record CreateRentalCommand(
    [property: JsonPropertyName("identificador")] string Identifier,
    [property: JsonPropertyName("entregador_id")] string MotoboyIdentifier,
    [property: JsonPropertyName("moto_id")] string MotoIdentifier,
    [property: JsonPropertyName("data_inicio")] DateTime DateStart,
    [property: JsonPropertyName("data_termino")] DateTime DateEnd,
    [property: JsonPropertyName("data_previsao_termino")] DateTime DatePreview,
    [property: JsonPropertyName("plano")] EPlan Plan) : IRequest<Result<RentalDto>>;