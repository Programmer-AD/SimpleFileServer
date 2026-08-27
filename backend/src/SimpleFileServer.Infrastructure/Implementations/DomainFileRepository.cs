using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Infrastructure.Implementations;

#warning "Not implemened"
internal class DomainFileRepository : IDomainFileRepository
{
    public Task<Guid> CreateAsync(DomainFile domainFile) => throw new NotImplementedException();
    public Task<DomainFile?> GetAsync(Guid id) => Task.FromResult((DomainFile?)null);
    public Task<List<DomainFile>> GetAllAsync() => throw new NotImplementedException();
    public Task UpdateAsync(DomainFile domainFile) => throw new NotImplementedException();
    public Task DeleteAsync(Guid id) => throw new NotImplementedException();
}
