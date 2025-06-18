using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when the service is currently too busy to service this request.
/// </summary>
public class BettingServiceBusyException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BettingServiceBusyException"/> class.
    /// </summary>
    public BettingServiceBusyException()
        : base("The service is currently too busy to service this request.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingServiceBusyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BettingServiceBusyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingServiceBusyException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BettingServiceBusyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingServiceBusyException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingServiceBusyException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingServiceBusyException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingServiceBusyException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
