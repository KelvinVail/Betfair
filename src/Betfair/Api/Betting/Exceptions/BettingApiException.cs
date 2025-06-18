using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Base exception class for all Betting API errors.
/// </summary>
public abstract class BettingApiException : HttpRequestException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BettingApiException"/> class.
    /// </summary>
    protected BettingApiException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingApiException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected BettingApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingApiException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected BettingApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingApiException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    protected BettingApiException(string message, HttpStatusCode statusCode)
        : base(message, null, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingApiException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    protected BettingApiException(string message, Exception innerException, HttpStatusCode statusCode)
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
