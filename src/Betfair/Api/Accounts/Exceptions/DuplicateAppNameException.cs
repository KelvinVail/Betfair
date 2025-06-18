using System.Net;

namespace Betfair.Api.Accounts.Exceptions;

/// <summary>
/// Exception thrown when a duplicate application name is provided.
/// </summary>
public class DuplicateAppNameException : AccountApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateAppNameException"/> class.
    /// </summary>
    public DuplicateAppNameException()
        : base("Duplicate application name.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateAppNameException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DuplicateAppNameException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateAppNameException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DuplicateAppNameException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateAppNameException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public DuplicateAppNameException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateAppNameException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public DuplicateAppNameException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
