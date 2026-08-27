using Microsoft.Extensions.Options;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Infrastructure.Configs;

namespace SimpleFileServer.Infrastructure.Implementations;

#warning "Not implemened"
internal class FileStore(
    IOptions<FileStoreOptions> options
) : IFileStore
{
    public Task<string> StoreAsync(Stream file)
        => throw new NotImplementedException();

    public Task<Stream?> GetContentAsync(string location) => throw new NotImplementedException();
    public Task DeleteAsync(string location) => throw new NotImplementedException();
}
