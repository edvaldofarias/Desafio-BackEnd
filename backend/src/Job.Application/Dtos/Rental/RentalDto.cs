using System.Text.Json.Serialization;

namespace Job.Application.Dtos.Rental;

public sealed record RentalDto(
    [property: JsonPropertyName("identificador")] string Identifier,
    [property: JsonPropertyName("valor_diaria")] decimal DailyValue,
    [property: JsonPropertyName("entregador_id")] string MotoboyIdentifier,
    [property: JsonPropertyName("moto_id")] string MotoIdentifier,
    [property: JsonPropertyName("data_inicio")] DateTime DateStart,
    [property: JsonPropertyName("data_termino")] DateTime DateEnd,
    [property: JsonPropertyName("data_previsao_termino")] DateTime DatePreview,
    [property: JsonPropertyName("data_devolucao")] DateTime? DateReturn,
    [property: JsonPropertyName("valor_total")] decimal? TotalValue = null,
    [property: JsonPropertyName("multa")] decimal? Fine = null);