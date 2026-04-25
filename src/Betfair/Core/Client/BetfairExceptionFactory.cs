using System.Net;
using Betfair.Api.Accounts.Exceptions;
using Betfair.Api.Betting.Exceptions;

namespace Betfair.Core.Client;

internal static class BetfairExceptionFactory
{
    // Error code constants to avoid string literal duplication
    private const string InvalidInputData = "INVALID_INPUT_DATA";
    private const string InvalidSessionInformation = "INVALID_SESSION_INFORMATION";
    private const string NoAppKey = "NO_APP_KEY";
    private const string NoSession = "NO_SESSION";
    private const string InvalidAppKey = "INVALID_APP_KEY";
    private const string UnexpectedError = "UNEXPECTED_ERROR";
    private const string TooManyRequests = "TOO_MANY_REQUESTS";
    private const string ServiceBusy = "SERVICE_BUSY";
    private const string TimeoutError = "TIMEOUT_ERROR";

    public static Exception CreateBettingException(BetfairErrorResponse errorResponse, HttpStatusCode statusCode)
    {
        var apiException = errorResponse.Detail.APINGException;
        var errorCode = apiException.ErrorCode;
        var message = EnhanceErrorMessage(GetDefaultBettingMessage(errorCode), errorResponse, apiException);

        BettingApiException exception = errorCode switch
        {
            "TOO_MUCH_DATA" => new TooMuchDataException(message, statusCode),
            InvalidInputData => new BettingInvalidInputDataException(message, statusCode),
            InvalidSessionInformation => new BettingInvalidSessionInformationException(message, statusCode),
            NoAppKey => new BettingNoAppKeyException(message, statusCode),
            NoSession => new BettingNoSessionException(message, statusCode),
            InvalidAppKey => new BettingInvalidAppKeyException(message, statusCode),
            "ACCESS_DENIED" => new AccessDeniedException(message, statusCode),
            UnexpectedError => new BettingUnexpectedErrorException(message, statusCode),
            TooManyRequests => new BettingTooManyRequestsException(message, statusCode),
            ServiceBusy => new BettingServiceBusyException(message, statusCode),
            TimeoutError => new BettingTimeoutErrorException(message, statusCode),
            "REQUEST_SIZE_EXCEEDS_LIMIT" => new RequestSizeExceedsLimitException(message, statusCode),
            "INVALID_JSON" => new InvalidJsonException(message, statusCode),
            "METHOD_NOT_FOUND" => new MethodNotFoundException(message, statusCode),
            "INVALID_PARAMETERS" => new InvalidParametersException(message, statusCode),
            "INTERNAL_JSON_RPC_ERROR" => new InternalJsonRpcErrorException(message, statusCode),
            _ => new BettingUnexpectedErrorException(message, statusCode)
        };

        exception.ErrorDetails = apiException.ErrorDetails;
        exception.RequestUUID = apiException.RequestUUID;
        return exception;
    }

    public static Exception CreateAccountException(BetfairErrorResponse errorResponse, HttpStatusCode statusCode)
    {
        var apiException = errorResponse.Detail.APINGException;
        var errorCode = apiException.ErrorCode;
        var message = EnhanceErrorMessage(GetDefaultAccountMessage(errorCode), errorResponse, apiException);

        AccountApiException exception = errorCode switch
        {
            InvalidSessionInformation => new InvalidSessionInformationException(message, statusCode),
            NoSession => new NoSessionException(message, statusCode),
            NoAppKey => new NoAppKeyException(message, statusCode),
            InvalidAppKey => new InvalidAppKeyException(message, statusCode),
            InvalidInputData => new InvalidInputDataException(message, statusCode),
            "INVALID_CLIENT_REF" => new InvalidClientRefException(message, statusCode),
            ServiceBusy => new ServiceBusyException(message, statusCode),
            TimeoutError => new TimeoutErrorException(message, statusCode),
            UnexpectedError => new UnexpectedErrorException(message, statusCode),
            TooManyRequests => new TooManyRequestsException(message, statusCode),
            "CUSTOMER_ACCOUNT_CLOSED" => new CustomerAccountClosedException(message, statusCode),
            "DUPLICATE_APP_NAME" => new DuplicateAppNameException(message, statusCode),
            "SUBSCRIPTION_EXPIRED" => new SubscriptionExpiredException(message, statusCode),
            _ => new UnexpectedErrorException(message, statusCode)
        };

        exception.ErrorDetails = apiException.ErrorDetails;
        exception.RequestUUID = apiException.RequestUUID;
        return exception;
    }

    private static string EnhanceErrorMessage(string defaultMessage, BetfairErrorResponse errorResponse, BetfairApiNgError apiException)
    {
        var message = defaultMessage;
        var betfairMessage = errorResponse.FaultString;
        var errorCode = apiException.ErrorCode;

        if (!string.IsNullOrEmpty(betfairMessage))
            message += $" Betfair error: {betfairMessage}";

        if (!string.IsNullOrEmpty(errorCode))
            message += $" (Error code: {errorCode})";

        return message;
    }

    private static string GetDefaultBettingMessage(string? errorCode) =>
        errorCode switch
        {
            "TOO_MUCH_DATA" => new TooMuchDataException().Message,
            InvalidInputData => new BettingInvalidInputDataException().Message,
            InvalidSessionInformation => new BettingInvalidSessionInformationException().Message,
            NoAppKey => new BettingNoAppKeyException().Message,
            NoSession => new BettingNoSessionException().Message,
            InvalidAppKey => new BettingInvalidAppKeyException().Message,
            "ACCESS_DENIED" => new AccessDeniedException().Message,
            TooManyRequests => new BettingTooManyRequestsException().Message,
            ServiceBusy => new BettingServiceBusyException().Message,
            TimeoutError => new BettingTimeoutErrorException().Message,
            "REQUEST_SIZE_EXCEEDS_LIMIT" => new RequestSizeExceedsLimitException().Message,
            "INVALID_JSON" => new InvalidJsonException().Message,
            "METHOD_NOT_FOUND" => new MethodNotFoundException().Message,
            "INVALID_PARAMETERS" => new InvalidParametersException().Message,
            "INTERNAL_JSON_RPC_ERROR" => new InternalJsonRpcErrorException().Message,
            _ => new BettingUnexpectedErrorException().Message
        };

    private static string GetDefaultAccountMessage(string? errorCode) =>
        errorCode switch
        {
            InvalidSessionInformation => new InvalidSessionInformationException().Message,
            NoSession => new NoSessionException().Message,
            NoAppKey => new NoAppKeyException().Message,
            InvalidAppKey => new InvalidAppKeyException().Message,
            InvalidInputData => new InvalidInputDataException().Message,
            "INVALID_CLIENT_REF" => new InvalidClientRefException().Message,
            ServiceBusy => new ServiceBusyException().Message,
            TimeoutError => new TimeoutErrorException().Message,
            TooManyRequests => new TooManyRequestsException().Message,
            "CUSTOMER_ACCOUNT_CLOSED" => new CustomerAccountClosedException().Message,
            "DUPLICATE_APP_NAME" => new DuplicateAppNameException().Message,
            "SUBSCRIPTION_EXPIRED" => new SubscriptionExpiredException().Message,
            _ => new UnexpectedErrorException().Message
        };
}
