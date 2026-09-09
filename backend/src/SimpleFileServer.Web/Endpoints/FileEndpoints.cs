using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Domain;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.Web.Endpoints;

internal static partial class FileEndpoints
{
    // Param has to be IFormFile to be bound, can't be part of model
    public static async Task<Ok<IdModel>> UploadAsync(IFormFile file, IDomainFileService fileService)
    {
        string fileName = file.FileName;
        if (!GetFileNameRegex().IsMatch(fileName))
        {
            fileName = GetCleanFileName(fileName);
        }

        using Stream fileStream = file.OpenReadStream();
        Guid id = await fileService.CreateAsync(fileName, fileStream);

        return TypedResults.Ok(new IdModel(id));
    }

    public static async Task<Ok<FileListModel>> GetAllAsync(IDomainFileService fileService)
    {
        List<DomainFile> files = await fileService.GetAllAsync();

        return TypedResults.Ok(new FileListModel([.. files.Select(x => x.ToFileInfoModel())]));
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

    private static string GetCleanFileName(string fileName)
    {
        var fileNameBuilder = new StringBuilder(fileName);
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileNameBuilder.Replace(invalidChar, '_');
        }

        return fileNameBuilder.ToString();
    }

    [GeneratedRegex(DomainEntityConstants.DomainFile_Name_Regex)]
    private static partial Regex GetFileNameRegex();
}
