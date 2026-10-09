using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Application.Abstractions.Services;

public interface IDomainFileService
{
    Task<Guid> RegisterAsync(string name);

    Task InitContentAsync(Guid id, Stream fileContent);

    Task<DomainFile> GetAsync(Guid id);

    Task<List<DomainFile>> GetAllAsync();

    Task<Stream> GetContentAsync(Guid id);

    Task RenameAsync(Guid id, string newName);

    Task DeleteAsync(Guid id);
}
