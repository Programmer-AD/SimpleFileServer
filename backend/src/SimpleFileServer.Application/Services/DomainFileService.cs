using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Domain.Exceptions;

namespace SimpleFileServer.Application.Services;

internal class DomainFileService(
    IDomainFileRepository domainFileRepository,
    IFileStore fileStore,
    TimeProvider timeProvider
) : IDomainFileService
{
    public async Task<Guid> RegisterAsync(string name, long size)
    {
        var domainFile = new DomainFile
        {
            Name = name,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            FileLocation = null,
            Size = size,
        };

        Guid id = await domainFileRepository.CreateAsync(domainFile);
        return id;
    }

    public async Task InitContentAsync(Guid id, Stream fileContent)
    {
        DomainFile domainFile = await GetAsync(id);
        if (!string.IsNullOrEmpty(domainFile.FileLocation))
        {
            throw new DomainException(
                DomainExceptionTypes.DomainFile_ContentAlreadyInitialized,
                $"The domain file \"{id}\" content is already initialized.");
        }

        domainFile.FileLocation = await fileStore.StoreAsync(fileContent);

        await domainFileRepository.UpdateAsync(domainFile);
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

        if (file.FileLocation == null)
        {
            throw new DomainException(
                DomainExceptionTypes.DomainFile_ContentNotInitialized,
                $"The domain file \"{id}\" content is not initialized.");
        }

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

        if (file.FileLocation != null)
        {
            fileStore.Delete(file.FileLocation);
        }

        await domainFileRepository.DeleteAsync(id);
    }
}
