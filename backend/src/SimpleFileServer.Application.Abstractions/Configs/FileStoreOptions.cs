using System.ComponentModel.DataAnnotations;

namespace SimpleFileServer.Application.Abstractions.Configs;

public class FileStoreOptions
{
    [Required]
    public string StorageFolderPath { get; set; } = null!;
}
