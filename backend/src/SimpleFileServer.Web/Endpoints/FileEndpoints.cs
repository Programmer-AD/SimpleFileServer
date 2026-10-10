using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.Web.Endpoints;

internal static class FileEndpoints
{
    public static async Task<Ok<IdModel>> RegisterAsync(FileRegisterRequest fileRegisterRequest, IDomainFileService fileService)
    {
        Guid id = await fileService.RegisterAsync(fileRegisterRequest.Name, fileRegisterRequest.Size);
        return TypedResults.Ok(new IdModel(id));
    }

    public static async Task<NoContent> InitContentAsync(Guid id, Stream fileContent, IDomainFileService fileService)
    {
        await fileService.InitContentAsync(id, fileContent);
        return TypedResults.NoContent();
    }

    public static async Task<Ok<FileListModel>> GetAllAsync(IDomainFileService fileService)
    {
        List<DomainFile> files = await fileService.GetAllAsync();

        return TypedResults.Ok(new FileListModel([.. files.Select(x => x.ToFileInfoModel())]));
    }

    public static async Task<Ok<FileInfoModel>> GetAsync(Guid id, IDomainFileService fileService)
    {
        DomainFile file = await fileService.GetAsync(id);
        return TypedResults.Ok(file.ToFileInfoModel());
    }

    public static async Task<FileStreamHttpResult> GetContentAsync(Guid id, IDomainFileService fileService)
    {
        DomainFile file = await fileService.GetAsync(id);
        Stream contentStream = await fileService.GetContentAsync(id);
        return TypedResults.File(
            contentStream,
            contentType: MediaTypeNames.Application.Octet,
            fileDownloadName: file.Name);
    }

    public static async Task<NoContent> RenameAsync(Guid id, FileRenameRequest request, IDomainFileService fileService)
    {
        await fileService.RenameAsync(id, request.NewName);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteAsync(Guid id, IDomainFileService fileService)
    {
        await fileService.DeleteAsync(id);
        return TypedResults.NoContent();
    }
}
