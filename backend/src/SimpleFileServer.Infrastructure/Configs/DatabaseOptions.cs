using System.ComponentModel.DataAnnotations;

namespace SimpleFileServer.Infrastructure.Configs;

public class DatabaseOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;
}
