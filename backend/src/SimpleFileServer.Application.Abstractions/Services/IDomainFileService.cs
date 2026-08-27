using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Application.Abstractions.Services;

public interface IDomainFileService
{
    Task<Guid> CreateAsync(string name, Stream createFile);

    Task<DomainFile> GetAsync(Guid id);

    Task<List<DomainFile>> GetAllAsync();

    Task<Stream> GetContentAsync(Guid id);

    Task RenameAsync(Guid id, string newName);

    Task DeleteAsync(Guid id);
}
