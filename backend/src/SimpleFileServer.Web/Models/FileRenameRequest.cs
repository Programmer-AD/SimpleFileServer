using System.ComponentModel.DataAnnotations;

namespace SimpleFileServer.Web.Models;

public record class FileRenameRequest(
    [Required]
    string NewName
);
