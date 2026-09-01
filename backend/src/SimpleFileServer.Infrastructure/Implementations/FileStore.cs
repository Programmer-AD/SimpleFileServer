using Microsoft.Extensions.Options;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Infrastructure.Configs;

namespace SimpleFileServer.Infrastructure.Implementations;

internal class FileStore(
    IOptions<FileStoreOptions> options
) : IFileStore
{
    public async Task<string> StoreAsync(Stream file)
    {
        string location = GetRandomFreeLocation();

        using FileStream stream = File.OpenWrite(location);
        await file.CopyToAsync(stream);

        return location;
    }

    public Stream? GetContent(string location)
    {
        if (!File.Exists(location))
        {
            return null;
        }

        return File.OpenRead(location);
    }

    public void Delete(string location)
    {
        File.Delete(location);
    }

    private string GetRandomFreeLocation()
    {
        string filePath;
        do
        {
            filePath = Path.Combine(options.Value.StorageFolderPath, Path.GetRandomFileName());
        } while (File.Exists(filePath));

        return filePath;
    }
}
