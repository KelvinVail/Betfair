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

        // Create exception with default message first, then enhance it
        BettingApiException exception = errorCode switch
        {
            "TOO_MUCH_DATA" => new TooMuchDataException(),
            InvalidInputData => new BettingInvalidInputDataException(),
            InvalidSessionInformation => new BettingInvalidSessionInformationException(),
            NoAppKey => new BettingNoAppKeyException(),
            NoSession => new BettingNoSessionException(),
            InvalidAppKey => new BettingInvalidAppKeyException(),
            "ACCESS_DENIED" => new AccessDeniedException(),
            UnexpectedError => new BettingUnexpectedErrorException(),
            TooManyRequests => new BettingTooManyRequestsException(),
            ServiceBusy => new BettingServiceBusyException(),
            TimeoutError => new BettingTimeoutErrorException(),
            "REQUEST_SIZE_EXCEEDS_LIMIT" => new RequestSizeExceedsLimitException(),
            "INVALID_JSON" => new InvalidJsonException(),
            "METHOD_NOT_FOUND" => new MethodNotFoundException(),
            "INVALID_PARAMETERS" => new InvalidParametersException(),
            "INTERNAL_JSON_RPC_ERROR" => new InternalJsonRpcErrorException(),
            _ => new BettingUnexpectedErrorException()
        };

        // Enhance the message with Betfair error details
        var enhancedMessage = EnhanceErrorMessage(exception.Message, errorResponse, apiException);

        // Create new exception with enhanced message and status code
        exception = CreateBettingExceptionWithEnhancedMessage(errorCode, enhancedMessage, statusCode);

        // Set the additional properties from the Betfair response
        exception.ErrorDetails = apiException.ErrorDetails;
        exception.RequestUUID = apiException.RequestUUID;
        return exception;
    }

    public static Exception CreateAccountException(BetfairErrorResponse errorResponse, HttpStatusCode statusCode)
    {
        var apiException = errorResponse.Detail.APINGException;
        var errorCode = apiException.ErrorCode;

        // Create exception with default message first, then enhance it
        AccountApiException exception = errorCode switch
        {
            InvalidSessionInformation => new InvalidSessionInformationException(),
            NoSession => new NoSessionException(),
            NoAppKey => new NoAppKeyException(),
            InvalidAppKey => new InvalidAppKeyException(),
            InvalidInputData => new InvalidInputDataException(),
            "INVALID_CLIENT_REF" => new InvalidClientRefException(),
            ServiceBusy => new ServiceBusyException(),
            TimeoutError => new TimeoutErrorException(),
            UnexpectedError => new UnexpectedErrorException(),
            TooManyRequests => new TooManyRequestsException(),
            "CUSTOMER_ACCOUNT_CLOSED" => new CustomerAccountClosedException(),
            "DUPLICATE_APP_NAME" => new DuplicateAppNameException(),
            "SUBSCRIPTION_EXPIRED" => new SubscriptionExpiredException(),
            _ => new UnexpectedErrorException()
        };

        // Enhance the message with Betfair error details
        var enhancedMessage = EnhanceErrorMessage(exception.Message, errorResponse, apiException);

        // Create new exception with enhanced message and status code
        exception = CreateAccountExceptionWithEnhancedMessage(errorCode, enhancedMessage, statusCode);

        // Set the additional properties from the Betfair response
        exception.ErrorDetails = apiException.ErrorDetails;
        exception.RequestUUID = apiException.RequestUUID;
        return exception;
    }

    private static string EnhanceErrorMessage(string originalMessage, BetfairErrorResponse errorResponse, BetfairApiNgError apiException)
    {
        var betfairMessage = errorResponse.FaultString;
        var errorCode = apiException.ErrorCode;

        // Start with the original descriptive message
        var enhancedMessage = originalMessage;

        // Add Betfair error details if available
        if (!string.IsNullOrEmpty(betfairMessage))
        {
            enhancedMessage += $" Betfair error: {betfairMessage}";
        }

        if (!string.IsNullOrEmpty(errorCode))
        {
            enhancedMessage += $" (Error code: {errorCode})";
        }

        return enhancedMessage;
    }

    private static BettingApiException CreateBettingExceptionWithEnhancedMessage(string? errorCode, string message, HttpStatusCode statusCode)
    {
        return errorCode switch
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
    }

    private static AccountApiException CreateAccountExceptionWithEnhancedMessage(string? errorCode, string message, HttpStatusCode statusCode)
    {
        return errorCode switch
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
    }
}
