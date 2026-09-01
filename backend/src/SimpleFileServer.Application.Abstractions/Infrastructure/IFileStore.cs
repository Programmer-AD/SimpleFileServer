namespace SimpleFileServer.Application.Abstractions.Infrastructure;

public interface IFileStore
{
    Task<string> StoreAsync(Stream file);

    Stream? GetContent(string location);

    void Delete(string location);
}
