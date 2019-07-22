namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstructionReportStatus
    {
        SUCCESS,
        FAILURE,
        TIMEOUT
    }
}