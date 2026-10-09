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

    /// <summary>
    ///     A generic type when action is invalid (e.g. due to entity state).
    ///     Should be used as prefix for more detailed errors.
    /// </summary>
    public const string GenericInvalidAction = $"{BaseUri}/invalid-action";

    public const string DomainFile_ContentNotInitialized = $"{GenericInvalidAction}/domain-file/content-not-inited";
    public const string DomainFile_ContentAlreadyInitialized = $"{GenericInvalidAction}/domain-file/content-already-inited";
}
