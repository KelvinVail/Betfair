using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when the operation requested too much data, exceeding the Market Data Request Limits.
/// You must adjust your request parameters to stay within the documented limits.
/// </summary>
public class TooMuchDataException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TooMuchDataException"/> class.
    /// </summary>
    public TooMuchDataException()
        : base("The operation requested too much data, exceeding the Market Data Request Limits. You must adjust your request parameters to stay within the documented limits.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooMuchDataException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TooMuchDataException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooMuchDataException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TooMuchDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooMuchDataException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public TooMuchDataException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooMuchDataException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public TooMuchDataException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
