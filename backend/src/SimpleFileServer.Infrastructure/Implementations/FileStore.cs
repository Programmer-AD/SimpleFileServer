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
        string path = GetFilePath(location);

        using FileStream stream = File.OpenWrite(path);
        await file.CopyToAsync(stream);

        return location;
    }

    public Stream? GetContent(string location)
    {
        string path = GetFilePath(location);
        if (!File.Exists(path))
        {
            return null;
        }

        return File.OpenRead(path);
    }

    public void Delete(string location)
    {
        string path = GetFilePath(location);
        File.Delete(path);
    }

    private string GetRandomFreeLocation()
    {
        string fileLocation;
        do
        {
            fileLocation = Path.GetRandomFileName();
        } while (File.Exists(GetFilePath(fileLocation)));

        return fileLocation;
    }

    private string GetFilePath(string location)
        => Path.Combine(options.Value.StorageFolderPath, location);
}
