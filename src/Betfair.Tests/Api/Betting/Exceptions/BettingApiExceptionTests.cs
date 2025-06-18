using System.Net;
using System.Reflection;
using Betfair.Api.Betting.Exceptions;

namespace Betfair.Tests.Api.Betting.Exceptions;

public class BettingApiExceptionTests
{
    private static readonly Dictionary<Type, string> ExpectedDefaultMessages = new ()
    {
        { typeof(TooMuchDataException), "The operation requested too much data, exceeding the Market Data Request Limits. You must adjust your request parameters to stay within the documented limits." },
        { typeof(BettingInvalidInputDataException), "Invalid input data. Please check the format of your request." },
        { typeof(BettingInvalidSessionInformationException), "The session token hasn't been provided, is invalid or has expired. You must login again to create a new session token." },
        { typeof(BettingNoAppKeyException), "An application key header ('X-Application') has not been provided in the request." },
        { typeof(BettingNoSessionException), "A session token header ('X-Authentication') has not been provided in the request." },
        { typeof(BettingInvalidAppKeyException), "The application key passed is invalid or is not present." },
        { typeof(AccessDeniedException), "Access denied to the requested operation." },
        { typeof(BettingUnexpectedErrorException), "An unexpected internal error occurred that prevented successful request processing." },
        { typeof(BettingTooManyRequestsException), "Too many requests. For more details relating to this error please see FAQ's." },
        { typeof(BettingServiceBusyException), "The service is currently too busy to service this request." },
        { typeof(BettingTimeoutErrorException), "The internal call to downstream service timed out." },
        { typeof(RequestSizeExceedsLimitException), "The request size exceeds the allowed limit." },
        { typeof(InvalidJsonException), "The JSON-RPC request is not valid JSON." },
        { typeof(MethodNotFoundException), "The method specified in the JSON-RPC request does not exist or is not available." },
        { typeof(InvalidParametersException), "The parameters specified in the JSON-RPC request are invalid." },
        { typeof(InternalJsonRpcErrorException), "An internal JSON-RPC error occurred." },
    };

    public static IEnumerable<object[]> GetBettingExceptionTypes()
    {
        return typeof(BettingApiException).Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BettingApiException)) && !t.IsAbstract)
            .Select(t => new object[] { t });
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveDefaultConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor(Type.EmptyTypes);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a parameterless constructor");

        var exception = (BettingApiException)Activator.CreateInstance(exceptionType) !;
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveCorrectDefaultMessage(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (BettingApiException)Activator.CreateInstance(exceptionType) !;

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
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveMessageConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message");

        const string customMessage = "Custom error message";
        var exception = (BettingApiException)Activator.CreateInstance(exceptionType, customMessage) !;

        exception.Message.Should().Be(customMessage);
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveMessageAndInnerExceptionConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string), typeof(Exception)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message and inner exception");

        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner exception");
        var exception = (BettingApiException)Activator.CreateInstance(exceptionType, message, innerException) !;

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveMessageAndStatusCodeConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string), typeof(HttpStatusCode)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message and HttpStatusCode");

        const string message = "Test message";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        var exception = (BettingApiException)Activator.CreateInstance(exceptionType, message, statusCode) !;

        exception.Message.Should().Be(message);
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveMessageInnerExceptionAndStatusCodeConstructor(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var constructor = exceptionType.GetConstructor([typeof(string), typeof(Exception), typeof(HttpStatusCode)]);

        constructor.Should().NotBeNull($"{exceptionType.Name} should have a constructor that takes a string message, inner exception, and HttpStatusCode");

        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner exception");
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        var exception = (BettingApiException)Activator.CreateInstance(exceptionType, message, innerException, statusCode) !;

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsInheritFromBettingApiException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (BettingApiException)Activator.CreateInstance(exceptionType) !;

        exception.Should().BeAssignableTo<BettingApiException>();
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsInheritFromHttpRequestException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (BettingApiException)Activator.CreateInstance(exceptionType) !;

        exception.Should().BeAssignableTo<HttpRequestException>();
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveErrorDetailsProperty(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (BettingApiException)Activator.CreateInstance(exceptionType) !;
        const string errorDetails = "Stack trace details";

        exception.ErrorDetails = errorDetails;

        exception.ErrorDetails.Should().Be(errorDetails);
    }

    [Theory]
    [MemberData(nameof(GetBettingExceptionTypes))]
    public void AllBettingExceptionsHaveRequestUUIDProperty(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        var exception = (BettingApiException)Activator.CreateInstance(exceptionType) !;
        const string requestUuid = "12345-67890-abcdef";

        exception.RequestUUID = requestUuid;

        exception.RequestUUID.Should().Be(requestUuid);
    }

    [Fact]
    public void AllBettingExceptionTypesAreDiscovered()
    {
        var discoveredTypes = GetBettingExceptionTypes().Select(data => (Type)data[0]).ToHashSet();
        var expectedTypes = ExpectedDefaultMessages.Keys.ToHashSet();

        discoveredTypes.Should().BeEquivalentTo(
            expectedTypes,
            "All betting exception types should be discovered by reflection and have expected default messages defined");
    }
}
