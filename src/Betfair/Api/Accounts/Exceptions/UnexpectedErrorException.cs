using System.Net;

namespace Betfair.Api.Accounts.Exceptions;

/// <summary>
/// Exception thrown when an unexpected internal error occurred that prevented successful request processing.
/// </summary>
public class UnexpectedErrorException : AccountApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedErrorException"/> class.
    /// </summary>
    public UnexpectedErrorException()
        : base("An unexpected internal error occurred that prevented successful request processing.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedErrorException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnexpectedErrorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedErrorException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnexpectedErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedErrorException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public UnexpectedErrorException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedErrorException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public UnexpectedErrorException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
