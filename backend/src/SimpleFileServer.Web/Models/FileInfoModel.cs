using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Web.Models;

public record class FileInfoModel(
    Guid Id,
    string Name,
    long Size,
    DateTime CreatedAt
);

public static class FileInfoModelExtensions
{
    public static FileInfoModel ToFileInfoModel(this DomainFile domainFile)
        => new(
            Id: domainFile.Id,
            Name: domainFile.Name,
            Size: domainFile.Size,
            CreatedAt: domainFile.CreatedAt
        );
}
