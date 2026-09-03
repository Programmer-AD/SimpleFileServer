using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Domain.Exceptions;

namespace SimpleFileServer.Application.Services;

internal class DomainFileService(
    IDomainFileRepository domainFileRepository,
    IFileStore fileStore
) : IDomainFileService
{
    public async Task<Guid> CreateAsync(string name, Stream createFile)
    {
        string location = await fileStore.StoreAsync(createFile);

        var domainFile = new DomainFile
        {
            Name = name,
            CreatedAt = DateTime.UtcNow,
            FileLocation = location,
            Size = createFile.Length,
        };

        Guid id = await domainFileRepository.CreateAsync(domainFile);
        return id;
    }

    public async Task<List<DomainFile>> GetAllAsync()
    {
        List<DomainFile> result = await domainFileRepository.GetAllAsync();
        return result;
    }

    public async Task<DomainFile> GetAsync(Guid id)
    {
        DomainFile file = await domainFileRepository.GetAsync(id)
            ?? throw DomainException.GenericNotFound("file", id);

        return file;
    }

    public async Task<Stream> GetContentAsync(Guid id)
    {
        DomainFile file = await GetAsync(id);

        Stream content = fileStore.GetContent(file.FileLocation)
            ?? throw DomainException.GenericNotFound("physical file", id);

        return content;
    }

    public async Task RenameAsync(Guid id, string newName)
    {
        DomainFile file = await GetAsync(id);

        file.Name = newName;

        await domainFileRepository.UpdateAsync(file);
    }

    public async Task DeleteAsync(Guid id)
    {
        DomainFile? file = await domainFileRepository.GetAsync(id);
        if (file == null)
        {
            return;
        }

        fileStore.Delete(file.FileLocation);

        await domainFileRepository.DeleteAsync(id);
    }
}
