using System.Text.Json.Serialization;
using Job.Application.Dtos.Rental;
using Job.Domain.Enums;

namespace Job.Application.Commands.Rental;

public sealed record CreateRentalCommand(Guid IdMoto, DateTime DatePreview, EPlan Plan) : IRequest<Result<RentalDto>>
{
    [JsonIgnore]
    public string Cnpj { get; set; } = string.Empty;
}