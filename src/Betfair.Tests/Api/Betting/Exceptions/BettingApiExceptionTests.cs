using System.Net;
using Betfair.Api.Betting.Exceptions;

namespace Betfair.Tests.Api.Betting.Exceptions;

public class BettingApiExceptionTests
{
    [Fact]
    public void TooMuchDataExceptionHasCorrectDefaultMessage()
    {
        var exception = new TooMuchDataException();

        exception.Message.Should().Be("The operation requested too much data, exceeding the Market Data Request Limits. You must adjust your request parameters to stay within the documented limits.");
    }

    [Fact]
    public void BettingInvalidInputDataExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingInvalidInputDataException();

        exception.Message.Should().Be("Invalid input data. Please check the format of your request.");
    }

    [Fact]
    public void BettingInvalidSessionInformationExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingInvalidSessionInformationException();

        exception.Message.Should().Be("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.");
    }

    [Fact]
    public void BettingNoAppKeyExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingNoAppKeyException();

        exception.Message.Should().Be("An application key header ('X-Application') has not been provided in the request.");
    }

    [Fact]
    public void BettingNoSessionExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingNoSessionException();

        exception.Message.Should().Be("A session token header ('X-Authentication') has not been provided in the request.");
    }

    [Fact]
    public void BettingInvalidAppKeyExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingInvalidAppKeyException();

        exception.Message.Should().Be("The application key passed is invalid or is not present.");
    }

    [Fact]
    public void AccessDeniedExceptionHasCorrectDefaultMessage()
    {
        var exception = new AccessDeniedException();

        exception.Message.Should().Be("Access denied to the requested operation.");
    }

    [Fact]
    public void BettingUnexpectedErrorExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingUnexpectedErrorException();

        exception.Message.Should().Be("An unexpected internal error occurred that prevented successful request processing.");
    }

    [Fact]
    public void BettingTooManyRequestsExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingTooManyRequestsException();

        exception.Message.Should().Be("Too many requests. For more details relating to this error please see FAQ's.");
    }

    [Fact]
    public void BettingServiceBusyExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingServiceBusyException();

        exception.Message.Should().Be("The service is currently too busy to service this request.");
    }

    [Fact]
    public void BettingTimeoutErrorExceptionHasCorrectDefaultMessage()
    {
        var exception = new BettingTimeoutErrorException();

        exception.Message.Should().Be("The internal call to downstream service timed out.");
    }

    [Fact]
    public void RequestSizeExceedsLimitExceptionHasCorrectDefaultMessage()
    {
        var exception = new RequestSizeExceedsLimitException();

        exception.Message.Should().Be("The request size exceeds the allowed limit.");
    }

    [Fact]
    public void InvalidJsonExceptionHasCorrectDefaultMessage()
    {
        var exception = new InvalidJsonException();

        exception.Message.Should().Be("The JSON-RPC request is not valid JSON.");
    }

    [Fact]
    public void MethodNotFoundExceptionHasCorrectDefaultMessage()
    {
        var exception = new MethodNotFoundException();

        exception.Message.Should().Be("The method specified in the JSON-RPC request does not exist or is not available.");
    }

    [Fact]
    public void InvalidParametersExceptionHasCorrectDefaultMessage()
    {
        var exception = new InvalidParametersException();

        exception.Message.Should().Be("The parameters specified in the JSON-RPC request are invalid.");
    }

    [Fact]
    public void InternalJsonRpcErrorExceptionHasCorrectDefaultMessage()
    {
        var exception = new InternalJsonRpcErrorException();

        exception.Message.Should().Be("An internal JSON-RPC error occurred.");
    }

    [Fact]
    public void ExceptionsCanBeCreatedWithCustomMessage()
    {
        const string customMessage = "Custom error message";

        var exception = new TooMuchDataException(customMessage);

        exception.Message.Should().Be(customMessage);
    }

    [Fact]
    public void ExceptionsCanBeCreatedWithInnerException()
    {
        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner exception");

        var exception = new TooMuchDataException(message, innerException);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void ExceptionsCanBeCreatedWithStatusCode()
    {
        const string message = "Test message";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

        var exception = new TooMuchDataException(message, statusCode);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void ExceptionsInheritFromBettingApiException()
    {
        var exception = new TooMuchDataException();

        exception.Should().BeAssignableTo<BettingApiException>();
    }

    [Fact]
    public void ExceptionsInheritFromHttpRequestException()
    {
        var exception = new TooMuchDataException();

        exception.Should().BeAssignableTo<HttpRequestException>();
    }

    [Fact]
    public void BettingApiExceptionHasErrorDetailsProperty()
    {
        var exception = new TooMuchDataException { ErrorDetails = "Stack trace details" };

        exception.ErrorDetails.Should().Be("Stack trace details");
    }

    [Fact]
    public void BettingApiExceptionHasRequestUUIDProperty()
    {
        var exception = new TooMuchDataException { RequestUUID = "12345-67890-abcdef" };

        exception.RequestUUID.Should().Be("12345-67890-abcdef");
    }
}
