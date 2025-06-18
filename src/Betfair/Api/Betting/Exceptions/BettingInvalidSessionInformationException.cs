using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when the session token hasn't been provided, is invalid or has expired.
/// You must login again to create a new session token.
/// </summary>
public class BettingInvalidSessionInformationException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidSessionInformationException"/> class.
    /// </summary>
    public BettingInvalidSessionInformationException()
        : base("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidSessionInformationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BettingInvalidSessionInformationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidSessionInformationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BettingInvalidSessionInformationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidSessionInformationException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingInvalidSessionInformationException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidSessionInformationException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingInvalidSessionInformationException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
