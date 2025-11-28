using Microsoft.Extensions.Options;
using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly FileStorageOptions _options;

    public LocalFileStorage(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<FileSaveResult> SaveAsync(string fileName, string contentType, Stream content, CancellationToken cancellationToken)
    {
        var root = EnsureRoot();
        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(root, storedFileName);
        var fullPath = Path.GetFullPath(relativePath);

        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        var fileInfo = new FileInfo(fullPath);
        return new FileSaveResult(relativePath.Replace('\\', '/'), fileInfo.Length, fileName, contentType);
    }

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(storagePath);
        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(storagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string EnsureRoot()
    {
        var root = string.IsNullOrWhiteSpace(_options.RootPath) ? "wwwroot/uploads" : _options.RootPath;
        var full = Path.GetFullPath(root);
        System.IO.Directory.CreateDirectory(full);
        return full;
    }
}
