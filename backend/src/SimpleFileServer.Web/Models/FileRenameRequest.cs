using System.ComponentModel.DataAnnotations;
using SimpleFileServer.Domain;

namespace SimpleFileServer.Web.Models;

public record class FileRenameRequest(
    [Required, MaxLength(DomainEntityConstants.DomainFile_Name_MaxLength)]
    string NewName
);
