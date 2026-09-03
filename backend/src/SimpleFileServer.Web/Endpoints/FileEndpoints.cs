using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.Web.Endpoints;

internal static class FileEndpoints
{
    // Param has to be IFormFile to be bound, can't be part of model
    public static async Task<Results<Ok<IdModel>, ValidationProblem>> UploadAsync(IFormFile file, IDomainFileService fileService)
    {
        string fileName = file.FileName;

        if (!IsFileNameValid(fileName, out string? validationProblem))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>()
            {
                ["fileName"] = [validationProblem],
            });
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

    public static async Task<Results<NoContent, ValidationProblem>> RenameAsync(Guid id, FileRenameRequest request, IDomainFileService fileService)
    {
        if (!IsFileNameValid(request.NewName, out string? validationProblem))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>()
            {
                ["fileName"] = [validationProblem],
            });
        }

        await fileService.RenameAsync(id, request.NewName);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteAsync(Guid id, IDomainFileService fileService)
    {
        await fileService.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    private static bool IsFileNameValid(string fileName, [MaybeNullWhen(true)] out string validationProblem)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            validationProblem = "Is empty or contains whitespace only characters.";
            return false;
        }

        char[] invalidChars = Path.GetInvalidFileNameChars();
        if (fileName.ContainsAny(invalidChars))
        {
            validationProblem = "Contains invalid characters.";
            return false;
        }

        validationProblem = null;
        return true;
    }
}
