using Betfair.Api.Accounts.Exceptions;
using Betfair.Api.Betting.Exceptions;
using Betfair.Core.Client;

namespace Betfair.Tests.Core.Client;

public class BetfairExceptionFactoryTests
{
    [Theory]
    [InlineData("TOO_MUCH_DATA", typeof(TooMuchDataException))]
    [InlineData("INVALID_INPUT_DATA", typeof(BettingInvalidInputDataException))]
    [InlineData("INVALID_SESSION_INFORMATION", typeof(BettingInvalidSessionInformationException))]
    [InlineData("NO_APP_KEY", typeof(BettingNoAppKeyException))]
    [InlineData("NO_SESSION", typeof(BettingNoSessionException))]
    [InlineData("INVALID_APP_KEY", typeof(BettingInvalidAppKeyException))]
    [InlineData("ACCESS_DENIED", typeof(AccessDeniedException))]
    [InlineData("UNEXPECTED_ERROR", typeof(BettingUnexpectedErrorException))]
    [InlineData("TOO_MANY_REQUESTS", typeof(BettingTooManyRequestsException))]
    [InlineData("SERVICE_BUSY", typeof(BettingServiceBusyException))]
    [InlineData("TIMEOUT_ERROR", typeof(BettingTimeoutErrorException))]
    [InlineData("REQUEST_SIZE_EXCEEDS_LIMIT", typeof(RequestSizeExceedsLimitException))]
    [InlineData("INVALID_JSON", typeof(InvalidJsonException))]
    [InlineData("METHOD_NOT_FOUND", typeof(MethodNotFoundException))]
    [InlineData("INVALID_PARAMETERS", typeof(InvalidParametersException))]
    [InlineData("INTERNAL_JSON_RPC_ERROR", typeof(InternalJsonRpcErrorException))]
    [InlineData("UNKNOWN_CODE", typeof(BettingUnexpectedErrorException))]
    public void CreateBettingExceptionReturnsCorrectType(string errorCode, Type expectedType)
    {
        var error = MakeBettingError(errorCode);

        var ex = BetfairExceptionFactory.CreateBettingException(error, HttpStatusCode.BadRequest);

        ex.Should().BeOfType(expectedType);
    }

    [Theory]
    [InlineData("INVALID_SESSION_INFORMATION", typeof(InvalidSessionInformationException))]
    [InlineData("NO_SESSION", typeof(NoSessionException))]
    [InlineData("NO_APP_KEY", typeof(NoAppKeyException))]
    [InlineData("INVALID_APP_KEY", typeof(Betfair.Api.Accounts.Exceptions.InvalidAppKeyException))]
    [InlineData("INVALID_INPUT_DATA", typeof(Betfair.Api.Accounts.Exceptions.InvalidInputDataException))]
    [InlineData("INVALID_CLIENT_REF", typeof(InvalidClientRefException))]
    [InlineData("SERVICE_BUSY", typeof(Betfair.Api.Accounts.Exceptions.ServiceBusyException))]
    [InlineData("TIMEOUT_ERROR", typeof(Betfair.Api.Accounts.Exceptions.TimeoutErrorException))]
    [InlineData("UNEXPECTED_ERROR", typeof(UnexpectedErrorException))]
    [InlineData("TOO_MANY_REQUESTS", typeof(Betfair.Api.Accounts.Exceptions.TooManyRequestsException))]
    [InlineData("CUSTOMER_ACCOUNT_CLOSED", typeof(CustomerAccountClosedException))]
    [InlineData("DUPLICATE_APP_NAME", typeof(DuplicateAppNameException))]
    [InlineData("SUBSCRIPTION_EXPIRED", typeof(SubscriptionExpiredException))]
    [InlineData("UNKNOWN_CODE", typeof(UnexpectedErrorException))]
    public void CreateAccountExceptionReturnsCorrectType(string errorCode, Type expectedType)
    {
        var error = MakeBettingError(errorCode);

        var ex = BetfairExceptionFactory.CreateAccountException(error, HttpStatusCode.BadRequest);

        ex.Should().BeOfType(expectedType);
    }

    [Fact]
    public void CreateBettingExceptionSetsErrorDetails()
    {
        var error = MakeBettingError("TOO_MUCH_DATA", errorDetails: "some details", requestUuid: "uuid-123");

        var ex = (BettingApiException)BetfairExceptionFactory.CreateBettingException(error, HttpStatusCode.BadRequest);

        ex.ErrorDetails.Should().Be("some details");
        ex.RequestUUID.Should().Be("uuid-123");
    }

    [Fact]
    public void CreateAccountExceptionSetsErrorDetails()
    {
        var error = MakeBettingError("NO_SESSION", errorDetails: "details", requestUuid: "uuid-456");

        var ex = (AccountApiException)BetfairExceptionFactory.CreateAccountException(error, HttpStatusCode.Forbidden);

        ex.ErrorDetails.Should().Be("details");
        ex.RequestUUID.Should().Be("uuid-456");
    }

    [Fact]
    public void CreateBettingExceptionIncludesFaultStringInMessage()
    {
        var error = MakeBettingError("TOO_MUCH_DATA", faultString: "DSC-0018");

        var ex = BetfairExceptionFactory.CreateBettingException(error, HttpStatusCode.BadRequest);

        ex.Message.Should().Contain("DSC-0018");
    }

    [Fact]
    public void CreateBettingExceptionIncludesErrorCodeInMessage()
    {
        var error = MakeBettingError("INVALID_JSON");

        var ex = BetfairExceptionFactory.CreateBettingException(error, HttpStatusCode.BadRequest);

        ex.Message.Should().Contain("INVALID_JSON");
    }

    [Fact]
    public void CreateAccountExceptionIncludesFaultStringInMessage()
    {
        var error = MakeBettingError("NO_SESSION", faultString: "Token expired");

        var ex = BetfairExceptionFactory.CreateAccountException(error, HttpStatusCode.Unauthorized);

        ex.Message.Should().Contain("Token expired");
    }

    private static BetfairErrorResponse MakeBettingError(
        string errorCode,
        string? faultString = null,
        string? errorDetails = null,
        string? requestUuid = null)
    {
        return new BetfairErrorResponse
        {
            FaultString = faultString,
            Detail = new BetfairErrorDetail
            {
                APINGException = new BetfairApiNgError
                {
                    ErrorCode = errorCode,
                    ErrorDetails = errorDetails,
                    RequestUUID = requestUuid,
                },
            },
        };
    }
}
