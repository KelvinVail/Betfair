namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExecutionReportErrorCode
    {
        SUCCESS,
        ERROR_IN_MATCHER,
        PROCESSED_WITH_ERRORS,
        BET_ACTION_ERROR,
        INVALID_ACCOUNT_STATE,
        INVALID_WALLET_STATUS,
        INSUFFICIENT_FUNDS,
        LOSS_LIMIT_EXCEEDED,
        MARKET_SUSPENDED,
        MARKET_NOT_OPEN_FOR_BETTING,
        DUPLICATE_TRANSACTION,
        INVALID_ORDER,
        INVALID_MARKET_ID,
        PERMISSION_DENIED,
        DUPLICATE_BETIDS,
        NO_ACTION_REQUIRED,
        SERVICE_UNAVAILABLE,
        REJECTED_BY_REGULATOR
    }
}