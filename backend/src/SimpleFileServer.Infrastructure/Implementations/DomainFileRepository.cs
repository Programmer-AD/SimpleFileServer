using Microsoft.EntityFrameworkCore;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Infrastructure.EF;

namespace SimpleFileServer.Infrastructure.Implementations;

internal class DomainFileRepository(
    AppDbContext appDbContext
) : IDomainFileRepository
{
    private DbSet<DomainFile> DomainFileSet => appDbContext.DomainFiles;

    public async Task<Guid> CreateAsync(DomainFile domainFile)
    {
        domainFile.Id = Guid.CreateVersion7();

        DomainFileSet.Add(domainFile);
        await appDbContext.SaveChangesAsync();

        return domainFile.Id;
    }

    public async Task<DomainFile?> GetAsync(Guid id)
    {
        DomainFile? result = await DomainFileSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return result;
    }

    public async Task<List<DomainFile>> GetAllAsync()
    {
        List<DomainFile> result = await DomainFileSet.AsNoTracking().ToListAsync();
        return result;
    }

    public async Task UpdateAsync(DomainFile domainFile)
    {
        DomainFileSet.Update(domainFile);
        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        DomainFile? domainFile = await DomainFileSet.FindAsync(id);
        if (domainFile == null)
        {
            return;
        }

        DomainFileSet.Remove(domainFile);
        await appDbContext.SaveChangesAsync();
    }
}
