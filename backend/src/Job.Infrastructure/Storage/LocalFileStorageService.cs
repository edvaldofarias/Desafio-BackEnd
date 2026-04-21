using Job.Application.Services;
using Microsoft.Extensions.Options;

namespace Job.Infrastructure.Storage;

public sealed class LocalFileStorageOptions
{
    public string RootPath { get; set; } = "uploads";
    public string PublicBaseUrl { get; set; } = "/uploads";
}

public sealed class LocalFileStorageService(IOptions<LocalFileStorageOptions> options) : IFileStorageService
{
    private readonly LocalFileStorageOptions _options = options.Value;

    public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(content);

        var rootPath = Path.IsPathRooted(_options.RootPath)
            ? _options.RootPath
            : Path.Combine(AppContext.BaseDirectory, _options.RootPath);

        Directory.CreateDirectory(rootPath);

        var extension = Path.GetExtension(fileName);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(rootPath, safeName);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{safeName}";
    }
}
