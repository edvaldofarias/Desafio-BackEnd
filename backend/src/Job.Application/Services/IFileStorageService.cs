namespace Job.Application.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken);
}
