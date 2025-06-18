using System.Net;

namespace Betfair.Api.Accounts.Exceptions;

/// <summary>
/// Exception thrown when the internal call to downstream service timed out.
/// </summary>
public class TimeoutErrorException : AccountApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutErrorException"/> class.
    /// </summary>
    public TimeoutErrorException()
        : base("The internal call to downstream service timed out.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutErrorException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TimeoutErrorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutErrorException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TimeoutErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutErrorException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public TimeoutErrorException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutErrorException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public TimeoutErrorException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
