using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when a session token header ('X-Authentication') has not been provided in the request.
/// Please note: The same error is returned by the Keep Alive operation if the X-Authentication header
/// is provided but the session value is invalid or if the session has expired.
/// </summary>
public class BettingNoSessionException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BettingNoSessionException"/> class.
    /// </summary>
    public BettingNoSessionException()
        : base("A session token header ('X-Authentication') has not been provided in the request.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingNoSessionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BettingNoSessionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingNoSessionException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BettingNoSessionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingNoSessionException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingNoSessionException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingNoSessionException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingNoSessionException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
