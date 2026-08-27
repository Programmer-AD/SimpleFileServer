namespace SimpleFileServer.Web.Models;

public record class FileInfoModel(
    Guid Id,
    string Name,
    long Size,
    DateTime CreatedAt
);
