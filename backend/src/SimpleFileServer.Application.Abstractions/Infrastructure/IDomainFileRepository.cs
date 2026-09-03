using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Application.Abstractions.Infrastructure;

public interface IDomainFileRepository
{
    Task<Guid> CreateAsync(DomainFile domainFile);

    Task<DomainFile?> GetAsync(Guid id);

    Task<List<DomainFile>> GetAllAsync();

    Task UpdateAsync(DomainFile domainFile);

    Task DeleteAsync(Guid id);
}
