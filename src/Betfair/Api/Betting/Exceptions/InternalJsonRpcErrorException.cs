using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when an internal JSON-RPC error occurred.
/// </summary>
public class InternalJsonRpcErrorException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalJsonRpcErrorException"/> class.
    /// </summary>
    public InternalJsonRpcErrorException()
        : base("An internal JSON-RPC error occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalJsonRpcErrorException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InternalJsonRpcErrorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalJsonRpcErrorException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InternalJsonRpcErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalJsonRpcErrorException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public InternalJsonRpcErrorException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalJsonRpcErrorException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public InternalJsonRpcErrorException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
