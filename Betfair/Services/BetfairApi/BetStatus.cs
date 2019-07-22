namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BetStatus
    {
        SETTLED,
        VOIDED,
        LAPSED,
        CANCELLED
    }
}