namespace SimpleFileServer.Domain.Exceptions;

/// <summary>
///     Represents exception in domain logic.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    ///     Shortcut for exception creation.
    /// </summary>
    /// <param name="entityType">Human readable entity type.</param>
    /// <param name="id">The id of entity which was searched.</param>
    /// <returns>Domain exception with "generic not found" type.</returns>
    public static DomainException GenericNotFound(string entityType, Guid id)
        => new(DomainExceptionTypes.GenericNotFound, $"The {entityType} with id \"{id}\" was not found.");

    /// <summary>
    ///     Shortcut for exception creation.
    /// </summary>
    /// <param name="details">Human readable details which can be part of response safely.</param>
    /// <returns>Domain exception with "generic access denied" type.</returns>
    public static DomainException GenericAccessDenied(string details)
        => new(DomainExceptionTypes.GenericAccessDenied, details);

    /// <summary>
    ///     The default constructor is defined mostly for unit-tests.
    ///     Direct usage is not recomended.
    /// </summary>
    public DomainException()
        : base("Unspecified domain exception have occured. Please review its location and consider using different constructor overload.")
    {
    }

    /// <summary>
    ///     Creates a domain exception with custom message and defined type/details.
    /// </summary>
    /// <param name="typeName">A type of domain exception. Should be value from <see cref="DomainExceptionTypes"/>.</param>
    /// <param name="details">Human readable details which can be part of response safely.</param>
    public DomainException(string typeName, string details)
        : base($"{details} [type=\"{typeName}\"].")
    {
        TypeName = typeName;
        Details = details;
    }

    /// <summary>
    ///     A type of domain exception.
    ///     Should be value from <see cref="DomainExceptionTypes"/>.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    ///     Human readable details which can be part of response safely.
    /// </summary>
    public string Details { get; } = string.Empty;
}
