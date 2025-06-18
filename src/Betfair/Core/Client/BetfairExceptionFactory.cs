using System.Net;
using Betfair.Api.Accounts.Exceptions;
using Betfair.Api.Betting.Exceptions;

namespace Betfair.Core.Client;

internal static class BetfairExceptionFactory
{
    public static Exception CreateBettingException(BetfairErrorResponse errorResponse, HttpStatusCode statusCode)
    {
        var apiException = errorResponse.Detail.APINGException;
        var errorCode = apiException.ErrorCode;

        // Create exception with default message first, then enhance it
        BettingApiException exception = errorCode switch
        {
            "TOO_MUCH_DATA" => new TooMuchDataException(),
            "INVALID_INPUT_DATA" => new BettingInvalidInputDataException(),
            "INVALID_SESSION_INFORMATION" => new BettingInvalidSessionInformationException(),
            "NO_APP_KEY" => new BettingNoAppKeyException(),
            "NO_SESSION" => new BettingNoSessionException(),
            "INVALID_APP_KEY" => new BettingInvalidAppKeyException(),
            "ACCESS_DENIED" => new AccessDeniedException(),
            "UNEXPECTED_ERROR" => new BettingUnexpectedErrorException(),
            "TOO_MANY_REQUESTS" => new BettingTooManyRequestsException(),
            "SERVICE_BUSY" => new BettingServiceBusyException(),
            "TIMEOUT_ERROR" => new BettingTimeoutErrorException(),
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
            "INVALID_SESSION_INFORMATION" => new InvalidSessionInformationException(),
            "NO_SESSION" => new NoSessionException(),
            "NO_APP_KEY" => new NoAppKeyException(),
            "INVALID_APP_KEY" => new InvalidAppKeyException(),
            "INVALID_INPUT_DATA" => new InvalidInputDataException(),
            "INVALID_CLIENT_REF" => new InvalidClientRefException(),
            "SERVICE_BUSY" => new ServiceBusyException(),
            "TIMEOUT_ERROR" => new TimeoutErrorException(),
            "UNEXPECTED_ERROR" => new UnexpectedErrorException(),
            "TOO_MANY_REQUESTS" => new TooManyRequestsException(),
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
            "INVALID_INPUT_DATA" => new BettingInvalidInputDataException(message, statusCode),
            "INVALID_SESSION_INFORMATION" => new BettingInvalidSessionInformationException(message, statusCode),
            "NO_APP_KEY" => new BettingNoAppKeyException(message, statusCode),
            "NO_SESSION" => new BettingNoSessionException(message, statusCode),
            "INVALID_APP_KEY" => new BettingInvalidAppKeyException(message, statusCode),
            "ACCESS_DENIED" => new AccessDeniedException(message, statusCode),
            "UNEXPECTED_ERROR" => new BettingUnexpectedErrorException(message, statusCode),
            "TOO_MANY_REQUESTS" => new BettingTooManyRequestsException(message, statusCode),
            "SERVICE_BUSY" => new BettingServiceBusyException(message, statusCode),
            "TIMEOUT_ERROR" => new BettingTimeoutErrorException(message, statusCode),
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
            "INVALID_SESSION_INFORMATION" => new InvalidSessionInformationException(message, statusCode),
            "NO_SESSION" => new NoSessionException(message, statusCode),
            "NO_APP_KEY" => new NoAppKeyException(message, statusCode),
            "INVALID_APP_KEY" => new InvalidAppKeyException(message, statusCode),
            "INVALID_INPUT_DATA" => new InvalidInputDataException(message, statusCode),
            "INVALID_CLIENT_REF" => new InvalidClientRefException(message, statusCode),
            "SERVICE_BUSY" => new ServiceBusyException(message, statusCode),
            "TIMEOUT_ERROR" => new TimeoutErrorException(message, statusCode),
            "UNEXPECTED_ERROR" => new UnexpectedErrorException(message, statusCode),
            "TOO_MANY_REQUESTS" => new TooManyRequestsException(message, statusCode),
            "CUSTOMER_ACCOUNT_CLOSED" => new CustomerAccountClosedException(message, statusCode),
            "DUPLICATE_APP_NAME" => new DuplicateAppNameException(message, statusCode),
            "SUBSCRIPTION_EXPIRED" => new SubscriptionExpiredException(message, statusCode),
            _ => new UnexpectedErrorException(message, statusCode)
        };
    }
}
