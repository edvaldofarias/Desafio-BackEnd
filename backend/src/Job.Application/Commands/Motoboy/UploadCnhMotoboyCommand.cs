using Microsoft.AspNetCore.Http;

namespace Job.Application.Commands.Motoboy;

public sealed record UploadCnhMotoboyCommand : IRequest<Result>
{
    public IFormFile FileDetails { get; init; } = default!;

    public string Cnpj { get; set; } = default!;
}