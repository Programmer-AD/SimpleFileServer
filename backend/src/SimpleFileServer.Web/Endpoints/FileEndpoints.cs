using Microsoft.AspNetCore.Http.HttpResults;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.Web.Endpoints;

internal static class FileEndpoints
{
    // Param has to be IFormFile to be bound, can't be part of model
    public static async Task<Created<IdModel>> UploadAsync(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public static async Task<Ok<FileListModel>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public static async Task<FileStreamHttpResult> DownloadAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public static async Task<NoContent> RenameAsync(Guid id, FileRenameRequest request)
    {
        throw new NotImplementedException();
    }

    public static async Task<NoContent> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
