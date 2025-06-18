using System.Net;

namespace Betfair.Api.Accounts.Exceptions;

/// <summary>
/// Base exception class for all Account API errors.
/// </summary>
public abstract class AccountApiException : HttpRequestException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountApiException"/> class.
    /// </summary>
    protected AccountApiException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountApiException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected AccountApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountApiException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected AccountApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountApiException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    protected AccountApiException(string message, HttpStatusCode statusCode)
        : base(message, null, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountApiException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    protected AccountApiException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }

    /// <summary>
    /// Gets the error details (stack trace) for this exception.
    /// </summary>
    public string? ErrorDetails { get; internal set; }

    /// <summary>
    /// Gets the request UUID for this exception.
    /// </summary>
    public string? RequestUUID { get; internal set; }
}
