using System.Text.Json.Serialization;
using Job.Domain.Enums;

namespace Job.Domain.Commands.Rent;

public sealed record CreateRentalCommand(Guid IdMoto, DateTime DatePreview, EPlan Plan)
{
    [JsonIgnore]
    public string Cnpj { get; set; } = default!;
}