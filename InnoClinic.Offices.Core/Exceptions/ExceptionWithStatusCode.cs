namespace InnoClinic.Offices.Core.Exceptions;

/// <summary>
/// Represents an exception with an associated HTTP status code.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExceptionWithStatusCode"/> class with a specified error message and HTTP status code.
/// </remarks>
/// <param name="message">The error message that describes the exception.</param>
/// <param name="httpStatusCode">The HTTP status code associated with the exception.</param>
public class ExceptionWithStatusCode(string message, int httpStatusCode) : Exception(message)
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int HttpStatusCode { get; } = httpStatusCode;
}