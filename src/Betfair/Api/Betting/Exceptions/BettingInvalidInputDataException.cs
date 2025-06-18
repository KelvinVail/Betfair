using System.Net;

namespace Betfair.Api.Betting.Exceptions;

/// <summary>
/// Exception thrown when invalid input data is provided. Please check the format of your request.
/// Please note: if the number of placeOrders, updateOrders, replaceOrders, or cancelOrders
/// instructions exceeds the documented limit you will also receive this error.
/// </summary>
public class BettingInvalidInputDataException : BettingApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidInputDataException"/> class.
    /// </summary>
    public BettingInvalidInputDataException()
        : base("Invalid input data. Please check the format of your request.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidInputDataException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BettingInvalidInputDataException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidInputDataException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BettingInvalidInputDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidInputDataException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingInvalidInputDataException(string message, HttpStatusCode statusCode)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BettingInvalidInputDataException"/> class with a specified error message, inner exception, and HTTP status code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public BettingInvalidInputDataException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException, statusCode)
    {
    }
}
