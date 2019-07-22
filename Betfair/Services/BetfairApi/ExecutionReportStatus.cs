namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExecutionReportStatus
    {
        SUCCESS,
        FAILURE,
        PROCESSED_WITH_ERRORS,
        TIMEOUT
    }
}