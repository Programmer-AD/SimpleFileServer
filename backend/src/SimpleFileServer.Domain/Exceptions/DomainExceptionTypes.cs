namespace SimpleFileServer.Domain.Exceptions;

/// <summary>
///     Types of domain exceptions in forms of URIs.
/// </summary>
public static class DomainExceptionTypes
{
    /// <summary>
    ///     General base URL, not to be used directly.
    /// </summary>
    private const string BaseUri = "/error";

    /// <summary>
    ///     A generic type for when entity is not found.
    ///     Can be used as prefix for more detailed errors.
    /// </summary>
    public const string GenericNotFound = $"{BaseUri}/not-found";

    /// <summary>
    ///     A generic type for when access is deined.
    ///     Can be used as prefix for more detailed errors.
    /// </summary>
    public const string GenericAccessDenied = $"{BaseUri}/access-denied";
}
