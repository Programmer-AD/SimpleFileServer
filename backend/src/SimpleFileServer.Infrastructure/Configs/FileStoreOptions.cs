using System.ComponentModel.DataAnnotations;

namespace SimpleFileServer.Infrastructure.Configs;

public class FileStoreOptions
{
    [Required]
    public string StorageFolderPath { get; set; } = null!;
}
