using System.Diagnostics.CodeAnalysis;

namespace SimpleFileServer.Domain;

public static class DomainEntityConstants
{
    public const int DomainFile_Name_MaxLength = 250;

    [StringSyntax("regex")]
    public const string DomainFile_Name_Regex = "^(?=\\s*\\S)[^\"<>|:*?\\/\t\n\v\f\r]+$";

    public const int DomainFile_FileLocation_MaxLength = 1000;
}
