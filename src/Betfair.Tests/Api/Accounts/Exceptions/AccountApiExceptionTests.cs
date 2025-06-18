using System.Net;
using Betfair.Api.Accounts.Exceptions;

namespace Betfair.Tests.Api.Accounts.Exceptions;

public class AccountApiExceptionTests
{
    [Fact]
    public void InvalidSessionInformationExceptionHasCorrectDefaultMessage()
    {
        var exception = new InvalidSessionInformationException();

        exception.Message.Should().Be("The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token.");
    }

    [Fact]
    public void NoSessionExceptionHasCorrectDefaultMessage()
    {
        var exception = new NoSessionException();

        exception.Message.Should().Be("A session token header ('X-Authentication') has not been provided in the request.");
    }

    [Fact]
    public void NoAppKeyExceptionHasCorrectDefaultMessage()
    {
        var exception = new NoAppKeyException();

        exception.Message.Should().Be("An application key header ('X-Application') has not been provided in the request.");
    }

    [Fact]
    public void InvalidAppKeyExceptionHasCorrectDefaultMessage()
    {
        var exception = new InvalidAppKeyException();

        exception.Message.Should().Be("The application key passed is invalid or is not present.");
    }

    [Fact]
    public void InvalidInputDataExceptionHasCorrectDefaultMessage()
    {
        var exception = new InvalidInputDataException();

        exception.Message.Should().Be("Invalid input data. Please check the format of your request.");
    }

    [Fact]
    public void ServiceBusyExceptionHasCorrectDefaultMessage()
    {
        var exception = new ServiceBusyException();

        exception.Message.Should().Be("The service is currently too busy to service this request.");
    }

    [Fact]
    public void TimeoutErrorExceptionHasCorrectDefaultMessage()
    {
        var exception = new TimeoutErrorException();

        exception.Message.Should().Be("The internal call to downstream service timed out.");
    }

    [Fact]
    public void UnexpectedErrorExceptionHasCorrectDefaultMessage()
    {
        var exception = new UnexpectedErrorException();

        exception.Message.Should().Be("An unexpected internal error occurred that prevented successful request processing.");
    }

    [Fact]
    public void TooManyRequestsExceptionHasCorrectDefaultMessage()
    {
        var exception = new TooManyRequestsException();

        exception.Message.Should().Be("Too many requests. For more details relating to this error please see FAQ's.");
    }

    [Fact]
    public void CustomerAccountClosedExceptionHasCorrectDefaultMessage()
    {
        var exception = new CustomerAccountClosedException();

        exception.Message.Should().Be("A token could not be created because the customer's account is CLOSED.");
    }

    [Fact]
    public void DuplicateAppNameExceptionHasCorrectDefaultMessage()
    {
        var exception = new DuplicateAppNameException();

        exception.Message.Should().Be("Duplicate application name.");
    }

    [Fact]
    public void SubscriptionExpiredExceptionHasCorrectDefaultMessage()
    {
        var exception = new SubscriptionExpiredException();

        exception.Message.Should().Be("An application key is required for this operation.");
    }

    [Fact]
    public void ExceptionsCanBeCreatedWithCustomMessage()
    {
        const string customMessage = "Custom error message";

        var exception = new InvalidSessionInformationException(customMessage);

        exception.Message.Should().Be(customMessage);
    }

    [Fact]
    public void ExceptionsCanBeCreatedWithInnerException()
    {
        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner exception");

        var exception = new InvalidSessionInformationException(message, innerException);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void ExceptionsCanBeCreatedWithStatusCode()
    {
        const string message = "Test message";
        const HttpStatusCode statusCode = HttpStatusCode.Unauthorized;

        var exception = new InvalidSessionInformationException(message, statusCode);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void ExceptionsInheritFromAccountApiException()
    {
        var exception = new InvalidSessionInformationException();

        exception.Should().BeAssignableTo<AccountApiException>();
    }

    [Fact]
    public void ExceptionsInheritFromHttpRequestException()
    {
        var exception = new InvalidSessionInformationException();

        exception.Should().BeAssignableTo<HttpRequestException>();
    }
}
