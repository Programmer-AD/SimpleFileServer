namespace SimpleFileServer.Application.Abstractions.Infrastructure;

public interface IFileStore
{
    Task<string> StoreAsync(Stream file);

    Task<Stream?> GetContentAsync(string location);

    Task DeleteAsync(string location);
}
