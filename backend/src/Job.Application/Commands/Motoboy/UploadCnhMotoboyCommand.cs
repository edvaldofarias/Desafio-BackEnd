using System.Text.Json.Serialization;

namespace Job.Application.Commands.Motoboy;

public sealed record UploadCnhMotoboyCommand : IRequest<Result>
{
    [JsonIgnore]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("imagem_cnh")]
    public string CnhImage { get; init; } = string.Empty;
}