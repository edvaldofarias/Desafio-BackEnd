using System.Text.Json.Serialization;
using Job.Application.Dtos.Rental;

namespace Job.Application.Commands.Rental;

public sealed record CancelRentalCommand(
    [property: JsonIgnore] string Identifier,
    [property: JsonPropertyName("data_devolucao")] DateTime DateReturn) : IRequest<Result<RentalDto>>;