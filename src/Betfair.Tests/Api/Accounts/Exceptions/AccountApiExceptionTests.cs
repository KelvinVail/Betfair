using System.Net;
using System.Reflection;
using Betfair.Api.Accounts.Exceptions;

namespace Betfair.Tests.Api.Accounts.Exceptions;

public class AccountApiExceptionTests
{
    private static readonly Dictionary<Type, string> ExpectedDefaultMessages = new ()
    {
        { typeof(InvalidSessionInformationException), "The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token." },
        { typeof(NoSessionException), "A session token header ('X-Authentication') has not been provided in the request." },
        { typeof(NoAppKeyException), "An application key header ('X-Application') has not been provided in the request." },
        { typeof(InvalidAppKeyException), "The application key passed is invalid or is not present." },
        { typeof(InvalidInputDataException), "Invalid input data. Please check the format of your request." },
        { typeof(InvalidClientRefException), "Invalid length for the client reference." },
        { typeof(ServiceBusyException), "The service is currently too busy to service this request." },
        { typeof(TimeoutErrorException), "The internal call to downstream service timed out." },
        { typeof(UnexpectedErrorException), "An unexpected internal error occurred that prevented successful request processing." },
        { typeof(TooManyRequestsException), "Too many requests. For more details relating to this error please see FAQ's." },
        { typeof(CustomerAccountClosedException), "A token could not be created because the customer's account is CLOSED." },
        { typeof(DuplicateAppNameException), "Duplicate application name." },
        { typeof(SubscriptionExpiredException), "An application key is required for this operation." },
    };

    public static IEnumerable<object[]> GetAccountExceptionTypes()
    {
        return typeof(AccountApiException).Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(AccountApiException)) && !t.IsAbstract)
            .Select(t => new object[] { t });
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveDefaultConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor(Type.EmptyTypes);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a parameterless constructor");

        var exception = (AccountApiException)Activator.CreateInstance(exceptionType) !;
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveCorrectDefaultMessage(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (AccountApiException)Activator.CreateInstance(exceptionType) !;

        if (ExpectedDefaultMessages.TryGetValue(exceptionType, out var expectedMessage))
        {
            exception.Message.Should().Be(expectedMessage);
        }
        else
        {
            exception.Message.Should().NotBeNullOrEmpty($"{exceptionType.Name} should have a non-empty default message");
        }
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveMessageConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message");

        const string customMessage = "Custom error message";
        var exception = (AccountApiException)Activator.CreateInstance(exceptionType, customMessage) !;

        exception.Message.Should().Be(customMessage);
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveMessageAndInnerExceptionConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string), typeof(Exception)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message and inner exception");

        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner exception");
        var exception = (AccountApiException)Activator.CreateInstance(exceptionType, message, innerException) !;

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveMessageAndStatusCodeConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string), typeof(HttpStatusCode)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message and HttpStatusCode");

        const string message = "Test message";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        var exception = (AccountApiException)Activator.CreateInstance(exceptionType, message, statusCode) !;

        exception.Message.Should().Be(message);
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveMessageInnerExceptionAndStatusCodeConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string), typeof(Exception), typeof(HttpStatusCode)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message, inner exception, and HttpStatusCode");

        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner exception");
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        var exception = (AccountApiException)Activator.CreateInstance(exceptionType, message, innerException, statusCode) !;

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsInheritFromAccountApiException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (AccountApiException)Activator.CreateInstance(exceptionType) !;

        exception.Should().BeAssignableTo<AccountApiException>();
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsInheritFromHttpRequestException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (AccountApiException)Activator.CreateInstance(exceptionType) !;

        exception.Should().BeAssignableTo<HttpRequestException>();
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveErrorDetailsProperty(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (AccountApiException)Activator.CreateInstance(exceptionType) !;
        const string errorDetails = "Stack trace details";

        exception.ErrorDetails = errorDetails;

        exception.ErrorDetails.Should().Be(errorDetails);
    }

    [Theory]
    [MemberData(nameof(GetAccountExceptionTypes))]
    public void AllAccountExceptionsHaveRequestUUIDProperty(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (AccountApiException)Activator.CreateInstance(exceptionType) !;
        const string requestUuid = "12345-67890-abcdef";

        exception.RequestUUID = requestUuid;

        exception.RequestUUID.Should().Be(requestUuid);
    }

    [Fact]
    public void AllAccountExceptionTypesAreDiscovered()
    {
        var discoveredTypes = GetAccountExceptionTypes().Select(data => (Type)data[0]).ToHashSet();
        var expectedTypes = ExpectedDefaultMessages.Keys.ToHashSet();

        discoveredTypes.Should().BeEquivalentTo(
            expectedTypes,
            "All account exception types should be discovered by reflection and have expected default messages defined");
    }
}
