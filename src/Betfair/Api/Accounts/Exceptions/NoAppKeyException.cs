using System.Net;

namespace Betfair.Api.Accounts.Exceptions;

/// <summary>
/// Exception thrown when an application key header ('X-Application') has not been provided in the request.
/// </summary>
public class NoAppKeyException : AccountApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoAppKeyException"/> class.
    /// </summary>
    public NoAppKeyException()
        : base("An application key header ('X-Application') has not been provided in the request.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoAppKeyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoAppKeyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoAppKeyException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NoAppKeyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoAppKeyException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public NoAppKeyException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoAppKeyException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public NoAppKeyException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
