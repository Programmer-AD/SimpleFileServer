namespace SimpleFileServer.Domain.Entities;

// Called as "DomainFile" to avoid naming conflicts with .NET types
public class DomainFile
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required long Size { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string FileLocation { get; set; }
}
