using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when access is denied to the requested operation.
/// </summary>
public class AccessDeniedException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
    /// </summary>
    public AccessDeniedException()
        : base("Access denied to the requested operation.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessDeniedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AccessDeniedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessDeniedException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AccessDeniedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessDeniedException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public AccessDeniedException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessDeniedException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public AccessDeniedException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
